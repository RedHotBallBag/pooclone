using LaunchBox.Core.Models;
using LaunchBox.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LaunchBox.Data.Repositories;

public class GameRepository : Repository<Game>, IGameRepository
{
    public GameRepository(LaunchBoxDbContext context) : base(context)
    {
    }

    public override async Task<Game?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(g => g.Platform)
            .Include(g => g.PreferredEmulator)
            .Include(g => g.Tags)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<Game?> GetByIdWithDetailsAsync(int id)
    {
        return await GetByIdAsync(id);
    }

    public override async Task<IEnumerable<Game>> GetAllAsync()
    {
        return await _dbSet
            .Include(g => g.Platform)
            .Include(g => g.PreferredEmulator)
            .OrderBy(g => g.Title)
            .ToListAsync();
    }

    public async Task<IEnumerable<Game>> GetByPlatformAsync(int platformId)
    {
        return await _dbSet
            .Include(g => g.Platform)
            .Include(g => g.PreferredEmulator)
            .Where(g => g.PlatformId == platformId)
            .OrderBy(g => g.Title)
            .ToListAsync();
    }

    public async Task<IEnumerable<Game>> GetFavoritesAsync()
    {
        return await _dbSet
            .Include(g => g.Platform)
            .Include(g => g.PreferredEmulator)
            .Where(g => g.IsFavorite)
            .OrderBy(g => g.Title)
            .ToListAsync();
    }

    public async Task<IEnumerable<Game>> GetRecentlyPlayedAsync(int count = 20)
    {
        return await _dbSet
            .Include(g => g.Platform)
            .Include(g => g.PreferredEmulator)
            .Where(g => g.LastPlayedAt != null)
            .OrderByDescending(g => g.LastPlayedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Game>> SearchAsync(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        return await _dbSet
            .Include(g => g.Platform)
            .Include(g => g.PreferredEmulator)
            .Where(g => g.Title.ToLower().Contains(lowerSearchTerm) ||
                       (g.Developer != null && g.Developer.ToLower().Contains(lowerSearchTerm)) ||
                       (g.Publisher != null && g.Publisher.ToLower().Contains(lowerSearchTerm)))
            .OrderBy(g => g.Title)
            .ToListAsync();
    }
}
