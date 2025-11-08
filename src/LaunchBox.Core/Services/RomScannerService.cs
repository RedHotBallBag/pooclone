using LaunchBox.Core.Models;

namespace LaunchBox.Core.Services;

public class RomScannerService : IRomScannerService
{
    private readonly Dictionary<int, string[]> _platformExtensions = new()
    {
        { 1, new[] { ".nes", ".unif", ".fds" } }, // NES
        { 2, new[] { ".sfc", ".smc" } }, // SNES
        { 3, new[] { ".z64", ".n64", ".v64" } }, // N64
        { 4, new[] { ".bin", ".cue", ".iso", ".img" } }, // PlayStation
        { 5, new[] { ".iso", ".bin", ".img" } }, // PlayStation 2
        { 6, new[] { ".md", ".bin", ".gen" } }, // Genesis
        { 7, new[] { ".gb", ".gbc" } }, // Game Boy
        { 8, new[] { ".gba" } }, // Game Boy Advance
        { 9, new[] { ".nds" } }, // Nintendo DS
        { 10, new[] { ".zip" } } // Arcade
    };

    public async Task<IEnumerable<Game>> ScanFolderAsync(string folderPath, int platformId)
    {
        if (!Directory.Exists(folderPath))
        {
            throw new DirectoryNotFoundException($"Folder not found: {folderPath}");
        }

        var extensions = GetSupportedExtensions(platformId);
        var games = new List<Game>();

        await Task.Run(() =>
        {
            var files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                .Where(f => extensions.Contains(Path.GetExtension(f).ToLower()));

            foreach (var file in files)
            {
                var game = CreateGameFromFile(file, platformId);
                games.Add(game);
            }
        });

        return games;
    }

    public IEnumerable<string> GetSupportedExtensions(int platformId)
    {
        if (_platformExtensions.TryGetValue(platformId, out var extensions))
        {
            return extensions;
        }
        return new[] { ".iso", ".bin", ".zip", ".7z" }; // Default extensions
    }

    private Game CreateGameFromFile(string filePath, int platformId)
    {
        var fileName = Path.GetFileNameWithoutExtension(filePath);

        // Parse metadata from filename (basic implementation)
        var title = CleanGameTitle(fileName);

        return new Game
        {
            Title = title,
            SortTitle = title,
            FilePath = filePath,
            PlatformId = platformId,
            CreatedAt = DateTime.UtcNow
        };
    }

    private string CleanGameTitle(string fileName)
    {
        // Remove common ROM tags like (USA), [!], etc.
        var title = fileName;

        // Remove content in parentheses and brackets
        title = System.Text.RegularExpressions.Regex.Replace(title, @"\([^)]*\)", "");
        title = System.Text.RegularExpressions.Regex.Replace(title, @"\[[^\]]*\]", "");

        // Clean up extra spaces
        title = System.Text.RegularExpressions.Regex.Replace(title, @"\s+", " ").Trim();

        return title;
    }
}
