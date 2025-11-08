using LaunchBox.Core.Models;
using LaunchBox.Data.Repositories;

namespace LaunchBox.Core.Services;

public class GameLibraryService : IGameLibraryService
{
    private readonly IGameRepository _gameRepository;
    private readonly IRomScannerService _romScanner;

    public GameLibraryService(IGameRepository gameRepository, IRomScannerService romScanner)
    {
        _gameRepository = gameRepository;
        _romScanner = romScanner;
    }

    public async Task<IEnumerable<Game>> GetAllGamesAsync()
    {
        return await _gameRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Game>> GetGamesByPlatformAsync(int platformId)
    {
        return await _gameRepository.GetByPlatformAsync(platformId);
    }

    public async Task<IEnumerable<Game>> GetFavoriteGamesAsync()
    {
        return await _gameRepository.GetFavoritesAsync();
    }

    public async Task<IEnumerable<Game>> GetRecentlyPlayedGamesAsync(int count = 20)
    {
        return await _gameRepository.GetRecentlyPlayedAsync(count);
    }

    public async Task<IEnumerable<Game>> SearchGamesAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllGamesAsync();

        return await _gameRepository.SearchAsync(searchTerm);
    }

    public async Task<Game?> GetGameByIdAsync(int id)
    {
        return await _gameRepository.GetByIdWithDetailsAsync(id);
    }

    public async Task<Game> AddGameAsync(Game game)
    {
        game.CreatedAt = DateTime.UtcNow;
        return await _gameRepository.AddAsync(game);
    }

    public async Task UpdateGameAsync(Game game)
    {
        game.UpdatedAt = DateTime.UtcNow;
        await _gameRepository.UpdateAsync(game);
    }

    public async Task DeleteGameAsync(int id)
    {
        var game = await _gameRepository.GetByIdAsync(id);
        if (game != null)
        {
            await _gameRepository.DeleteAsync(game);
        }
    }

    public async Task ToggleFavoriteAsync(int gameId)
    {
        var game = await _gameRepository.GetByIdAsync(gameId);
        if (game != null)
        {
            game.IsFavorite = !game.IsFavorite;
            await UpdateGameAsync(game);
        }
    }

    public async Task UpdatePlayStatsAsync(int gameId)
    {
        var game = await _gameRepository.GetByIdAsync(gameId);
        if (game != null)
        {
            game.LastPlayedAt = DateTime.UtcNow;
            game.PlayCount++;
            await UpdateGameAsync(game);
        }
    }

    public async Task<IEnumerable<Game>> ScanRomFolderAsync(string folderPath, int platformId)
    {
        var scannedGames = await _romScanner.ScanFolderAsync(folderPath, platformId);

        // Add games that don't already exist
        var existingGames = await _gameRepository.GetByPlatformAsync(platformId);
        var existingPaths = existingGames.Select(g => g.FilePath).ToHashSet();

        var newGames = scannedGames.Where(g => !existingPaths.Contains(g.FilePath)).ToList();

        if (newGames.Any())
        {
            await _gameRepository.AddRangeAsync(newGames);
        }

        return newGames;
    }
}
