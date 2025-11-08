using LaunchBox.Core.Models;

namespace LaunchBox.Core.Services;

public interface IGameLauncherService
{
    Task<bool> LaunchGameAsync(Game game, Emulator? emulator = null);
    Task<bool> ValidateEmulatorAsync(Emulator emulator);
    Task<bool> ValidateGameFileAsync(Game game);
}
