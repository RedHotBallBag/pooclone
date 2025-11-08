using LaunchBox.Core.Models;
using System.Diagnostics;

namespace LaunchBox.Core.Services;

public class GameLauncherService : IGameLauncherService
{
    private readonly IEmulatorService _emulatorService;
    private readonly IGameLibraryService _gameLibraryService;

    public GameLauncherService(IEmulatorService emulatorService, IGameLibraryService gameLibraryService)
    {
        _emulatorService = emulatorService;
        _gameLibraryService = gameLibraryService;
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
            Process.Start(startInfo);

            // Update play statistics
            await _gameLibraryService.UpdatePlayStatsAsync(game.Id);

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
