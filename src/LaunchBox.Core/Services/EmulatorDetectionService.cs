using LaunchBox.Core.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace LaunchBox.Core.Services;

public class EmulatorDetectionService : IEmulatorDetectionService
{
    private readonly Dictionary<string, EmulatorDefinition> _knownEmulators = new()
    {
        ["RetroArch"] = new EmulatorDefinition
        {
            Name = "RetroArch",
            ExecutableNames = new[] { "retroarch.exe" },
            CommonPaths = new[]
            {
                @"C:\RetroArch",
                @"C:\Program Files\RetroArch",
                @"C:\Program Files (x86)\RetroArch",
                Environment.ExpandEnvironmentVariables(@"%APPDATA%\RetroArch")
            },
            VersionCommand = "--version"
        },
        ["PCSX2"] = new EmulatorDefinition
        {
            Name = "PCSX2",
            ExecutableNames = new[] { "pcsx2.exe", "pcsx2-qt.exe" },
            CommonPaths = new[]
            {
                @"C:\Program Files\PCSX2",
                @"C:\Program Files (x86)\PCSX2",
                @"C:\Emulators\PCSX2"
            },
            VersionCommand = "--version"
        },
        ["Dolphin"] = new EmulatorDefinition
        {
            Name = "Dolphin",
            ExecutableNames = new[] { "Dolphin.exe" },
            CommonPaths = new[]
            {
                @"C:\Program Files\Dolphin-x64",
                @"C:\Program Files (x86)\Dolphin-x64",
                @"C:\Emulators\Dolphin"
            }
        },
        ["Project64"] = new EmulatorDefinition
        {
            Name = "Project64",
            ExecutableNames = new[] { "Project64.exe" },
            CommonPaths = new[]
            {
                @"C:\Program Files\Project64",
                @"C:\Program Files (x86)\Project64",
                @"C:\Emulators\Project64"
            }
        },
        ["ePSXe"] = new EmulatorDefinition
        {
            Name = "ePSXe",
            ExecutableNames = new[] { "ePSXe.exe" },
            CommonPaths = new[]
            {
                @"C:\ePSXe",
                @"C:\Program Files\ePSXe",
                @"C:\Emulators\ePSXe"
            }
        },
        ["DeSmuME"] = new EmulatorDefinition
        {
            Name = "DeSmuME",
            ExecutableNames = new[] { "DeSmuME.exe", "DeSmuME_x64.exe" },
            CommonPaths = new[]
            {
                @"C:\Program Files\DeSmuME",
                @"C:\Program Files (x86)\DeSmuME",
                @"C:\Emulators\DeSmuME"
            }
        },
        ["RPCS3"] = new EmulatorDefinition
        {
            Name = "RPCS3",
            ExecutableNames = new[] { "rpcs3.exe" },
            CommonPaths = new[]
            {
                @"C:\Program Files\RPCS3",
                @"C:\Emulators\RPCS3"
            }
        },
        ["Cemu"] = new EmulatorDefinition
        {
            Name = "Cemu",
            ExecutableNames = new[] { "Cemu.exe" },
            CommonPaths = new[]
            {
                @"C:\Program Files\Cemu",
                @"C:\Emulators\Cemu"
            }
        }
    };

    public async Task<IEnumerable<Emulator>> ScanForEmulatorsAsync()
    {
        var detectedEmulators = new List<Emulator>();

        foreach (var definition in _knownEmulators.Values)
        {
            var emulator = await DetectEmulatorFromDefinitionAsync(definition);
            if (emulator != null)
            {
                detectedEmulators.Add(emulator);
            }
        }

        return detectedEmulators;
    }

    public async Task<Emulator?> DetectEmulatorAsync(string emulatorName)
    {
        if (_knownEmulators.TryGetValue(emulatorName, out var definition))
        {
            return await DetectEmulatorFromDefinitionAsync(definition);
        }

        return null;
    }

    public IEnumerable<string> GetSearchPaths()
    {
        var searchPaths = new List<string>
        {
            @"C:\Emulators",
            @"C:\Program Files",
            @"C:\Program Files (x86)",
            Environment.ExpandEnvironmentVariables(@"%APPDATA%")
        };

        return searchPaths;
    }

    public async Task<EmulatorValidationResult> ValidateEmulatorAsync(Emulator emulator)
    {
        var result = new EmulatorValidationResult();

        // Check if executable exists
        if (!File.Exists(emulator.ExecutablePath))
        {
            result.Issues.Add($"Emulator executable not found: {emulator.ExecutablePath}");
            result.IsValid = false;
            return result;
        }

        // Check working directory if specified
        if (!string.IsNullOrEmpty(emulator.WorkingDirectory) && !Directory.Exists(emulator.WorkingDirectory))
        {
            result.Issues.Add($"Working directory not found: {emulator.WorkingDirectory}");
        }

        // Try to get version
        try
        {
            result.Version = await GetEmulatorVersionAsync(emulator.ExecutablePath);
        }
        catch
        {
            result.Issues.Add("Could not determine emulator version");
        }

        result.IsValid = result.Issues.Count == 0;
        return result;
    }

    private async Task<Emulator?> DetectEmulatorFromDefinitionAsync(EmulatorDefinition definition)
    {
        // Search in common paths
        foreach (var path in definition.CommonPaths)
        {
            if (!Directory.Exists(path)) continue;

            foreach (var exeName in definition.ExecutableNames)
            {
                var fullPath = Path.Combine(path, exeName);
                if (File.Exists(fullPath))
                {
                    return new Emulator
                    {
                        Name = definition.Name,
                        ExecutablePath = fullPath,
                        WorkingDirectory = path,
                        IsPreset = false,
                        UseQuotesForPath = true
                    };
                }
            }
        }

        // Search in Program Files recursively (limited depth)
        var programFilesPaths = new[]
        {
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
        };

        foreach (var basePath in programFilesPaths)
        {
            if (!Directory.Exists(basePath)) continue;

            foreach (var exeName in definition.ExecutableNames)
            {
                var foundPath = await SearchForFileAsync(basePath, exeName, maxDepth: 2);
                if (foundPath != null)
                {
                    return new Emulator
                    {
                        Name = definition.Name,
                        ExecutablePath = foundPath,
                        WorkingDirectory = Path.GetDirectoryName(foundPath),
                        IsPreset = false,
                        UseQuotesForPath = true
                    };
                }
            }
        }

        return null;
    }

    private async Task<string?> SearchForFileAsync(string directory, string fileName, int maxDepth, int currentDepth = 0)
    {
        if (currentDepth > maxDepth) return null;

        try
        {
            var filePath = Path.Combine(directory, fileName);
            if (File.Exists(filePath))
                return filePath;

            foreach (var subDir in Directory.GetDirectories(directory))
            {
                var found = await SearchForFileAsync(subDir, fileName, maxDepth, currentDepth + 1);
                if (found != null)
                    return found;
            }
        }
        catch
        {
            // Ignore access denied etc.
        }

        return null;
    }

    private async Task<string?> GetEmulatorVersionAsync(string executablePath)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                Arguments = "--version",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null) return null;

            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            // Try to extract version from output
            var versionMatch = Regex.Match(output, @"(\d+\.\d+(?:\.\d+)?)");
            return versionMatch.Success ? versionMatch.Groups[1].Value : null;
        }
        catch
        {
            return null;
        }
    }

    private class EmulatorDefinition
    {
        public required string Name { get; init; }
        public required string[] ExecutableNames { get; init; }
        public required string[] CommonPaths { get; init; }
        public string? VersionCommand { get; init; }
    }
}
