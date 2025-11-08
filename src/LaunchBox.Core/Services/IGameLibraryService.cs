using LaunchBox.Core.Models;

namespace LaunchBox.Core.Services;

public interface IGameLibraryService
{
    Task<IEnumerable<Game>> GetAllGamesAsync();
    Task<IEnumerable<Game>> GetGamesByPlatformAsync(int platformId);
    Task<IEnumerable<Game>> GetFavoriteGamesAsync();
    Task<IEnumerable<Game>> GetRecentlyPlayedGamesAsync(int count = 20);
    Task<IEnumerable<Game>> SearchGamesAsync(string searchTerm);
    Task<Game?> GetGameByIdAsync(int id);
    Task<Game> AddGameAsync(Game game);
    Task UpdateGameAsync(Game game);
    Task DeleteGameAsync(int id);
    Task ToggleFavoriteAsync(int gameId);
    Task UpdatePlayStatsAsync(int gameId);
    Task<IEnumerable<Game>> ScanRomFolderAsync(string folderPath, int platformId);
}
