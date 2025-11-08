using LaunchBox.Core.Models;

namespace LaunchBox.Core.Repositories;

public interface IGameRepository : IRepository<Game>
{
    Task<IEnumerable<Game>> GetByPlatformAsync(int platformId);
    Task<IEnumerable<Game>> GetFavoritesAsync();
    Task<IEnumerable<Game>> GetRecentlyPlayedAsync(int count = 20);
    Task<IEnumerable<Game>> SearchAsync(string searchTerm);
    Task<Game?> GetByIdWithDetailsAsync(int id);
}
