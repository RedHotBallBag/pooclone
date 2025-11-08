using LaunchBox.Core.Models;
using System.Net.Http;

namespace LaunchBox.Core.Services;

public class MediaScraperService : IMediaScraperService
{
    private readonly HttpClient _httpClient;
    private readonly string _mediaFolder;

    public MediaScraperService(string mediaFolder = "Media")
    {
        _httpClient = new HttpClient();
        _mediaFolder = mediaFolder;
        EnsureMediaFoldersExist();
    }

    private void EnsureMediaFoldersExist()
    {
        Directory.CreateDirectory(Path.Combine(_mediaFolder, "BoxArt"));
        Directory.CreateDirectory(Path.Combine(_mediaFolder, "Screenshots"));
        Directory.CreateDirectory(Path.Combine(_mediaFolder, "Backgrounds"));
    }

    public async Task<bool> DownloadBoxArtAsync(Game game)
    {
        var metadata = await SearchGameMetadataAsync(game.Title, game.PlatformId);
        if (metadata?.BoxArtUrl == null)
            return false;

        var fileName = $"{SanitizeFileName(game.Title)}_{game.Id}.jpg";
        var filePath = Path.Combine(_mediaFolder, "BoxArt", fileName);

        return await DownloadImageAsync(metadata.BoxArtUrl, filePath);
    }

    public async Task<bool> DownloadScreenshotAsync(Game game)
    {
        var metadata = await SearchGameMetadataAsync(game.Title, game.PlatformId);
        if (metadata?.ScreenshotUrl == null)
            return false;

        var fileName = $"{SanitizeFileName(game.Title)}_{game.Id}.jpg";
        var filePath = Path.Combine(_mediaFolder, "Screenshots", fileName);

        return await DownloadImageAsync(metadata.ScreenshotUrl, filePath);
    }

    public async Task<bool> DownloadBackgroundAsync(Game game)
    {
        var metadata = await SearchGameMetadataAsync(game.Title, game.PlatformId);
        if (metadata?.BackgroundUrl == null)
            return false;

        var fileName = $"{SanitizeFileName(game.Title)}_{game.Id}.jpg";
        var filePath = Path.Combine(_mediaFolder, "Backgrounds", fileName);

        return await DownloadImageAsync(metadata.BackgroundUrl, filePath);
    }

    public async Task<GameMetadata?> SearchGameMetadataAsync(string title, int platformId)
    {
        // This is a placeholder implementation
        // In a real implementation, you would integrate with APIs like:
        // - TheGamesDB API
        // - IGDB API
        // - ScreenScraper API
        // - Custom LaunchBox API

        await Task.Delay(100); // Simulate API call

        // For MVP, return null (manual metadata entry)
        return null;
    }

    public async Task<bool> UpdateGameMetadataAsync(Game game)
    {
        var metadata = await SearchGameMetadataAsync(game.Title, game.PlatformId);
        if (metadata == null)
            return false;

        // Update game with metadata
        if (!string.IsNullOrEmpty(metadata.Description))
            game.Description = metadata.Description;

        if (!string.IsNullOrEmpty(metadata.Developer))
            game.Developer = metadata.Developer;

        if (!string.IsNullOrEmpty(metadata.Publisher))
            game.Publisher = metadata.Publisher;

        if (metadata.ReleaseYear.HasValue)
            game.ReleaseYear = metadata.ReleaseYear;

        if (!string.IsNullOrEmpty(metadata.Genre))
            game.Genre = metadata.Genre;

        if (metadata.Rating.HasValue)
            game.Rating = metadata.Rating;

        return true;
    }

    private async Task<bool> DownloadImageAsync(string url, string filePath)
    {
        try
        {
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return false;

            var bytes = await response.Content.ReadAsByteArrayAsync();
            await File.WriteAllBytesAsync(filePath, bytes);

            return true;
        }
        catch
        {
            return false;
        }
    }

    private string SanitizeFileName(string fileName)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return string.Join("_", fileName.Split(invalid, StringSplitOptions.RemoveEmptyEntries));
    }
}
