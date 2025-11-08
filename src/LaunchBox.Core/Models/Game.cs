namespace LaunchBox.Core.Models;

/// <summary>
/// Represents a game in the library
/// </summary>
public class Game
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? SortTitle { get; set; }
    public string FilePath { get; set; } = string.Empty;

    // Metadata
    public int? ReleaseYear { get; set; }
    public string? Developer { get; set; }
    public string? Publisher { get; set; }
    public string? Genre { get; set; }
    public string? Description { get; set; }
    public decimal? Rating { get; set; }

    // Media paths
    public string? BoxArtPath { get; set; }
    public string? ScreenshotPath { get; set; }
    public string? BackgroundPath { get; set; }
    public string? VideoPath { get; set; }

    // Platform and emulator
    public int PlatformId { get; set; }
    public Platform Platform { get; set; } = null!;

    public int? PreferredEmulatorId { get; set; }
    public Emulator? PreferredEmulator { get; set; }

    // User data
    public bool IsFavorite { get; set; }
    public DateTime? LastPlayedAt { get; set; }
    public int PlayCount { get; set; }
    public int? PlayTime { get; set; } // in minutes

    // Tags
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
