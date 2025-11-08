namespace LaunchBox.Core.Models;

/// <summary>
/// Represents an emulator configuration
/// </summary>
public class Emulator
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ExecutablePath { get; set; } = string.Empty;
    public string? CommandLineArguments { get; set; }

    // Platform association
    public int? PlatformId { get; set; }
    public Platform? Platform { get; set; }

    // Configuration
    public bool IsDefault { get; set; }
    public string? WorkingDirectory { get; set; }
    public bool UseQuotesForPath { get; set; } = true;

    // Preset information
    public bool IsPreset { get; set; }
    public string? PresetName { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
