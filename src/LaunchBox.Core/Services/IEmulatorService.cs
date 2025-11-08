using LaunchBox.Core.Models;

namespace LaunchBox.Core.Services;

public interface IEmulatorService
{
    Task<IEnumerable<Emulator>> GetAllEmulatorsAsync();
    Task<IEnumerable<Emulator>> GetEmulatorsByPlatformAsync(int platformId);
    Task<Emulator?> GetEmulatorByIdAsync(int id);
    Task<Emulator?> GetDefaultEmulatorForPlatformAsync(int platformId);
    Task<Emulator> AddEmulatorAsync(Emulator emulator);
    Task UpdateEmulatorAsync(Emulator emulator);
    Task DeleteEmulatorAsync(int id);
    Task InitializeEmulatorPresetsAsync();
}
