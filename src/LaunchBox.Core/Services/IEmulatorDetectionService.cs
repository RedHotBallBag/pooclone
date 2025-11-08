using LaunchBox.Core.Models;

namespace LaunchBox.Core.Services;

/// <summary>
/// Service for auto-detecting installed emulators
/// </summary>
public interface IEmulatorDetectionService
{
    /// <summary>
    /// Scan common install paths for known emulators
    /// </summary>
    Task<IEnumerable<Emulator>> ScanForEmulatorsAsync();

    /// <summary>
    /// Detect specific emulator by name
    /// </summary>
    Task<Emulator?> DetectEmulatorAsync(string emulatorName);

    /// <summary>
    /// Get default search paths for emulators
    /// </summary>
    IEnumerable<string> GetSearchPaths();

    /// <summary>
    /// Validate emulator installation
    /// </summary>
    Task<EmulatorValidationResult> ValidateEmulatorAsync(Emulator emulator);
}

public class EmulatorValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Issues { get; set; } = new();
    public string? Version { get; set; }
}
