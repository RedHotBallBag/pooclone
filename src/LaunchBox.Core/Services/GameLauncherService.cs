using LaunchBox.Core.Models;
using LaunchBox.Core.Plugins;
using System.Diagnostics;

namespace LaunchBox.Core.Services;

public class GameLauncherService : IGameLauncherService
{
    private readonly IEmulatorService _emulatorService;
    private readonly IGameLibraryService _gameLibraryService;
    private IPluginManager? _pluginManager;

    public GameLauncherService(IEmulatorService emulatorService, IGameLibraryService gameLibraryService)
    {
        _emulatorService = emulatorService;
        _gameLibraryService = gameLibraryService;
    }

    public void SetPluginManager(IPluginManager pluginManager)
    {
        _pluginManager = pluginManager;
    }

    public async Task<bool> LaunchGameAsync(Game game, Emulator? emulator = null)
    {
        // Validate game file exists
        if (!await ValidateGameFileAsync(game))
        {
            throw new FileNotFoundException($"Game file not found: {game.FilePath}");
        }

        // Get emulator to use
        emulator ??= game.PreferredEmulator ?? await _emulatorService.GetDefaultEmulatorForPlatformAsync(game.PlatformId);

        if (emulator == null)
        {
            throw new InvalidOperationException($"No emulator configured for platform: {game.Platform.Name}");
        }

        // Validate emulator
        if (!await ValidateEmulatorAsync(emulator))
        {
            throw new FileNotFoundException($"Emulator not found: {emulator.ExecutablePath}");
        }

        // Build command line arguments
        var arguments = BuildCommandLineArguments(emulator, game.FilePath);

        // Launch the game
        var startInfo = new ProcessStartInfo
        {
            FileName = emulator.ExecutablePath,
            Arguments = arguments,
            UseShellExecute = true
        };

        if (!string.IsNullOrEmpty(emulator.WorkingDirectory))
        {
            startInfo.WorkingDirectory = emulator.WorkingDirectory;
        }

        try
        {
            // Call plugin hooks before launch
            if (_pluginManager != null)
            {
                foreach (var plugin in _pluginManager.GetLoadedPlugins())
                {
                    await plugin.OnGameLaunchAsync(game, emulator);
                }
            }

            var process = Process.Start(startInfo);

            // Update play statistics
            await _gameLibraryService.UpdatePlayStatsAsync(game.Id);

            // Monitor process for close event
            if (process != null)
            {
                _ = Task.Run(async () =>
                {
                    await process.WaitForExitAsync();

                    // Call plugin hooks after game closes
                    if (_pluginManager != null)
                    {
                        foreach (var plugin in _pluginManager.GetLoadedPlugins())
                        {
                            await plugin.OnGameCloseAsync(game);
                        }
                    }
                });
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to launch game: {ex.Message}", ex);
        }
    }

    public Task<bool> ValidateEmulatorAsync(Emulator emulator)
    {
        return Task.FromResult(File.Exists(emulator.ExecutablePath));
    }

    public Task<bool> ValidateGameFileAsync(Game game)
    {
        return Task.FromResult(File.Exists(game.FilePath));
    }

    private string BuildCommandLineArguments(Emulator emulator, string gamePath)
    {
        if (string.IsNullOrEmpty(emulator.CommandLineArguments))
        {
            return emulator.UseQuotesForPath ? $"\"{gamePath}\"" : gamePath;
        }

        // Replace {0} placeholder with game path
        var path = emulator.UseQuotesForPath ? gamePath : gamePath;
        return string.Format(emulator.CommandLineArguments, path);
    }
}
