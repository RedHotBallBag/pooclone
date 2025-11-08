namespace LaunchBox.Core.Services;

public class PlatformDetectionService : IPlatformDetectionService
{
    private readonly Dictionary<string, int> _extensionToPlatform = new(StringComparer.OrdinalIgnoreCase)
    {
        // NES
        [".nes"] = 1, [".unif"] = 1, [".fds"] = 1,

        // SNES
        [".sfc"] = 2, [".smc"] = 2, [".swc"] = 2,

        // N64
        [".z64"] = 3, [".n64"] = 3, [".v64"] = 3,

        // PlayStation
        [".cue"] = 4, [".chd"] = 4, [".pbp"] = 4,

        // PlayStation 2
        // .iso can be multiple platforms, handle specially

        // Genesis
        [".md"] = 6, [".gen"] = 6, [".smd"] = 6,

        // Game Boy
        [".gb"] = 7, [".gbc"] = 7,

        // GBA
        [".gba"] = 8,

        // Nintendo DS
        [".nds"] = 9,

        // Arcade
        [".zip"] = 10 // Archive formats need context
    };

    private readonly Dictionary<string, int> _folderNameToPlatform = new(StringComparer.OrdinalIgnoreCase)
    {
        ["NES"] = 1, ["Nintendo"] = 1, ["Famicom"] = 1,
        ["SNES"] = 2, ["Super Nintendo"] = 2,
        ["N64"] = 3, ["Nintendo 64"] = 3,
        ["PS1"] = 4, ["PSX"] = 4, ["PlayStation"] = 4,
        ["PS2"] = 5, ["PlayStation 2"] = 5,
        ["Genesis"] = 6, ["Mega Drive"] = 6, ["Sega"] = 6,
        ["GB"] = 7, ["Game Boy"] = 7, ["GBC"] = 7,
        ["GBA"] = 8, ["Game Boy Advance"] = 8,
        ["DS"] = 9, ["Nintendo DS"] = 9, ["NDS"] = 9,
        ["Arcade"] = 10, ["MAME"] = 10
    };

    private readonly Dictionary<int, string[]> _platformToExtensions = new()
    {
        [1] = new[] { ".nes", ".unif", ".fds" },
        [2] = new[] { ".sfc", ".smc", ".swc" },
        [3] = new[] { ".z64", ".n64", ".v64" },
        [4] = new[] { ".cue", ".bin", ".iso", ".chd", ".pbp" },
        [5] = new[] { ".iso", ".bin" },
        [6] = new[] { ".md", ".gen", ".smd", ".bin" },
        [7] = new[] { ".gb", ".gbc" },
        [8] = new[] { ".gba" },
        [9] = new[] { ".nds" },
        [10] = new[] { ".zip" }
    };

    public int? DetectPlatformFromFile(string filePath)
    {
        var extension = Path.GetExtension(filePath);

        // Try exact extension match first
        if (_extensionToPlatform.TryGetValue(extension, out var platformId))
        {
            // For ambiguous extensions like .iso or .bin, try folder detection
            if (extension.Equals(".iso", StringComparison.OrdinalIgnoreCase) ||
                extension.Equals(".bin", StringComparison.OrdinalIgnoreCase))
            {
                var folderPlatform = DetectPlatformFromFolder(Path.GetDirectoryName(filePath) ?? string.Empty);
                if (folderPlatform.HasValue)
                    return folderPlatform.Value;
            }

            return platformId;
        }

        // Try folder-based detection
        return DetectPlatformFromFolder(Path.GetDirectoryName(filePath) ?? string.Empty);
    }

    public int? DetectPlatformFromFolder(string folderPath)
    {
        if (string.IsNullOrEmpty(folderPath))
            return null;

        var folderName = Path.GetFileName(folderPath);

        // Try exact match
        if (_folderNameToPlatform.TryGetValue(folderName, out var platformId))
            return platformId;

        // Try partial match
        foreach (var kvp in _folderNameToPlatform)
        {
            if (folderName.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                return kvp.Value;
        }

        // Try parent folder
        var parentFolder = Path.GetDirectoryName(folderPath);
        if (!string.IsNullOrEmpty(parentFolder) && parentFolder != folderPath)
        {
            return DetectPlatformFromFolder(parentFolder);
        }

        return null;
    }

    public IEnumerable<string> GetPlatformExtensions(int platformId)
    {
        if (_platformToExtensions.TryGetValue(platformId, out var extensions))
            return extensions;

        return Array.Empty<string>();
    }
}
