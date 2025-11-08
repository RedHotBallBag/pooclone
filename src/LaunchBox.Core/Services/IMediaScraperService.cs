using LaunchBox.Core.Models;

namespace LaunchBox.Core.Services;

public interface IMediaScraperService
{
    Task<bool> DownloadBoxArtAsync(Game game);
    Task<bool> DownloadScreenshotAsync(Game game);
    Task<bool> DownloadBackgroundAsync(Game game);
    Task<GameMetadata?> SearchGameMetadataAsync(string title, int platformId);
    Task<bool> UpdateGameMetadataAsync(Game game);
}

public class GameMetadata
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Developer { get; set; }
    public string? Publisher { get; set; }
    public int? ReleaseYear { get; set; }
    public string? Genre { get; set; }
    public decimal? Rating { get; set; }
    public string? BoxArtUrl { get; set; }
    public string? ScreenshotUrl { get; set; }
    public string? BackgroundUrl { get; set; }
}
