using LaunchBox.Core.Models;
using LaunchBox.Data.Repositories;

namespace LaunchBox.Core.Services;

public class EmulatorService : IEmulatorService
{
    private readonly IRepository<Emulator> _emulatorRepository;

    public EmulatorService(IRepository<Emulator> emulatorRepository)
    {
        _emulatorRepository = emulatorRepository;
    }

    public async Task<IEnumerable<Emulator>> GetAllEmulatorsAsync()
    {
        return await _emulatorRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Emulator>> GetEmulatorsByPlatformAsync(int platformId)
    {
        return await _emulatorRepository.FindAsync(e => e.PlatformId == platformId);
    }

    public async Task<Emulator?> GetEmulatorByIdAsync(int id)
    {
        return await _emulatorRepository.GetByIdAsync(id);
    }

    public async Task<Emulator?> GetDefaultEmulatorForPlatformAsync(int platformId)
    {
        var emulators = await GetEmulatorsByPlatformAsync(platformId);
        return emulators.FirstOrDefault(e => e.IsDefault);
    }

    public async Task<Emulator> AddEmulatorAsync(Emulator emulator)
    {
        emulator.CreatedAt = DateTime.UtcNow;

        // If this is set as default, unset other defaults for the same platform
        if (emulator.IsDefault && emulator.PlatformId.HasValue)
        {
            var existingDefaults = await GetEmulatorsByPlatformAsync(emulator.PlatformId.Value);
            foreach (var existing in existingDefaults.Where(e => e.IsDefault))
            {
                existing.IsDefault = false;
                await _emulatorRepository.UpdateAsync(existing);
            }
        }

        return await _emulatorRepository.AddAsync(emulator);
    }

    public async Task UpdateEmulatorAsync(Emulator emulator)
    {
        emulator.UpdatedAt = DateTime.UtcNow;

        // If this is set as default, unset other defaults for the same platform
        if (emulator.IsDefault && emulator.PlatformId.HasValue)
        {
            var existingDefaults = await GetEmulatorsByPlatformAsync(emulator.PlatformId.Value);
            foreach (var existing in existingDefaults.Where(e => e.IsDefault && e.Id != emulator.Id))
            {
                existing.IsDefault = false;
                await _emulatorRepository.UpdateAsync(existing);
            }
        }

        await _emulatorRepository.UpdateAsync(emulator);
    }

    public async Task DeleteEmulatorAsync(int id)
    {
        var emulator = await _emulatorRepository.GetByIdAsync(id);
        if (emulator != null)
        {
            await _emulatorRepository.DeleteAsync(emulator);
        }
    }

    public async Task InitializeEmulatorPresetsAsync()
    {
        // Check if presets already exist
        var existingEmulators = await GetAllEmulatorsAsync();
        if (existingEmulators.Any(e => e.IsPreset))
            return;

        var presets = GetEmulatorPresets();
        await _emulatorRepository.AddRangeAsync(presets);
    }

    private List<Emulator> GetEmulatorPresets()
    {
        return new List<Emulator>
        {
            // RetroArch cores
            new Emulator
            {
                Name = "RetroArch - NES (Nestopia)",
                ExecutablePath = @"C:\RetroArch\retroarch.exe",
                CommandLineArguments = "-L cores\\nestopia_libretro.dll \"{0}\"",
                PlatformId = 1,
                IsPreset = true,
                PresetName = "RetroArch-NES",
                IsDefault = true
            },
            new Emulator
            {
                Name = "RetroArch - SNES (Snes9x)",
                ExecutablePath = @"C:\RetroArch\retroarch.exe",
                CommandLineArguments = "-L cores\\snes9x_libretro.dll \"{0}\"",
                PlatformId = 2,
                IsPreset = true,
                PresetName = "RetroArch-SNES",
                IsDefault = true
            },
            new Emulator
            {
                Name = "RetroArch - N64 (Mupen64Plus)",
                ExecutablePath = @"C:\RetroArch\retroarch.exe",
                CommandLineArguments = "-L cores\\mupen64plus_next_libretro.dll \"{0}\"",
                PlatformId = 3,
                IsPreset = true,
                PresetName = "RetroArch-N64",
                IsDefault = true
            },
            new Emulator
            {
                Name = "RetroArch - PlayStation (Beetle PSX)",
                ExecutablePath = @"C:\RetroArch\retroarch.exe",
                CommandLineArguments = "-L cores\\mednafen_psx_libretro.dll \"{0}\"",
                PlatformId = 4,
                IsPreset = true,
                PresetName = "RetroArch-PS1",
                IsDefault = true
            },
            new Emulator
            {
                Name = "RetroArch - Genesis (Genesis Plus GX)",
                ExecutablePath = @"C:\RetroArch\retroarch.exe",
                CommandLineArguments = "-L cores\\genesis_plus_gx_libretro.dll \"{0}\"",
                PlatformId = 6,
                IsPreset = true,
                PresetName = "RetroArch-Genesis",
                IsDefault = true
            },
            new Emulator
            {
                Name = "RetroArch - Game Boy (Gambatte)",
                ExecutablePath = @"C:\RetroArch\retroarch.exe",
                CommandLineArguments = "-L cores\\gambatte_libretro.dll \"{0}\"",
                PlatformId = 7,
                IsPreset = true,
                PresetName = "RetroArch-GB",
                IsDefault = true
            },
            new Emulator
            {
                Name = "RetroArch - GBA (mGBA)",
                ExecutablePath = @"C:\RetroArch\retroarch.exe",
                CommandLineArguments = "-L cores\\mgba_libretro.dll \"{0}\"",
                PlatformId = 8,
                IsPreset = true,
                PresetName = "RetroArch-GBA",
                IsDefault = true
            },
            // Standalone emulators
            new Emulator
            {
                Name = "Project64",
                ExecutablePath = @"C:\Emulators\Project64\Project64.exe",
                CommandLineArguments = "\"{0}\"",
                PlatformId = 3,
                IsPreset = true,
                PresetName = "Project64"
            },
            new Emulator
            {
                Name = "Dolphin",
                ExecutablePath = @"C:\Emulators\Dolphin\Dolphin.exe",
                CommandLineArguments = "-e \"{0}\"",
                PlatformId = null,
                IsPreset = true,
                PresetName = "Dolphin"
            },
            new Emulator
            {
                Name = "PCSX2",
                ExecutablePath = @"C:\Emulators\PCSX2\pcsx2.exe",
                CommandLineArguments = "\"{0}\" --fullscreen",
                PlatformId = 5,
                IsPreset = true,
                PresetName = "PCSX2"
            },
            new Emulator
            {
                Name = "ePSXe",
                ExecutablePath = @"C:\Emulators\ePSXe\ePSXe.exe",
                CommandLineArguments = "-nogui -loadbin \"{0}\"",
                PlatformId = 4,
                IsPreset = true,
                PresetName = "ePSXe"
            },
            new Emulator
            {
                Name = "DeSmuME",
                ExecutablePath = @"C:\Emulators\DeSmuME\DeSmuME.exe",
                CommandLineArguments = "\"{0}\"",
                PlatformId = 9,
                IsPreset = true,
                PresetName = "DeSmuME",
                IsDefault = true
            }
        };
    }
}
