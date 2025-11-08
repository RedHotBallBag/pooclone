namespace LaunchBox.Core.Models;

/// <summary>
/// Represents a gaming platform (e.g., NES, SNES, PS1, etc.)
/// </summary>
public class Platform
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Manufacturer { get; set; }
    public int? ReleaseYear { get; set; }
    public string? IconPath { get; set; }

    // Navigation properties
    public ICollection<Game> Games { get; set; } = new List<Game>();
    public ICollection<Emulator> Emulators { get; set; } = new List<Emulator>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
