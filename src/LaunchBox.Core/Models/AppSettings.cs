namespace LaunchBox.Core.Models;

/// <summary>
/// Application settings and user preferences
/// </summary>
public class AppSettings
{
    public int Id { get; set; }

    // UI preferences
    public string Theme { get; set; } = "Light";
    public ViewMode DefaultViewMode { get; set; } = ViewMode.Grid;
    public int GridItemSize { get; set; } = 200;

    // Startup behavior
    public bool StartMaximized { get; set; }
    public bool RememberLastFilter { get; set; } = true;
    public int? LastSelectedPlatformId { get; set; }

    // Media settings
    public bool AutoDownloadMetadata { get; set; } = true;
    public bool AutoDownloadMedia { get; set; } = true;
    public string MediaFolder { get; set; } = "Media";

    // Performance
    public bool EnableAnimations { get; set; } = true;
    public int MaxRecentGames { get; set; } = 20;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum ViewMode
{
    Grid,
    List,
    Details
}
