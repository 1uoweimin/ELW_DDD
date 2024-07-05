using ListeningService.Domain;
using ListeningService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ListeningService.Infrastructure;

public class ListeningRepository : IListeningRepository
{
    private readonly ListeningDbContext _dbCtx;
    public ListeningRepository(ListeningDbContext dbCtx) { _dbCtx = dbCtx; }

    public async Task<int> GetMaxSequenceOfCategoryAsync()
    {
        int? num = await _dbCtx.Category.AsNoTracking().MaxAsync(c => (int?)c.SequenceNumber);
        return num ?? 0;
    }
    public async Task<Category?> GetCategoryByIdAsync(Guid categoryId)
    {
        return await _dbCtx.Category.FindAsync(categoryId);
    }
    public Task<Category[]> GetCategorysAsync()
    {
        return _dbCtx.Category.OrderBy(c => c.SequenceNumber).ToArrayAsync();
    }

    public async Task<int> GetMaxSequenceOfAlbumAsync(Guid categoryId)
    {
        int? num = await _dbCtx.Album.AsNoTracking().Where(a => a.CategoryId == categoryId).MaxAsync(c => (int?)c.SequenceNumber);
        return num ?? 0;
    }
    public async Task<Album?> GetAlbumByIdAsync(Guid albumId)
    {
        return await _dbCtx.Album.FindAsync(albumId);
    }
    public Task<Album[]> GetAlbumsByCategoryIdAsync(Guid categoryId)
    {
        return _dbCtx.Album.Where(a => a.CategoryId == categoryId).OrderBy(a => a.SequenceNumber).ToArrayAsync();
    }

    public async Task<int> GetMaxSequenceOfEpisodeAsync(Guid albumId)
    {
        int? num = await _dbCtx.Episode.AsNoTracking().Where(e => e.AlbumId == albumId).MaxAsync(e => (int?)e.SequenceNumber);
        return num ?? 0;
    }
    public async Task<Episode?> GetEpisodeByIdAsync(Guid episodeId)
    {
        return await _dbCtx.Episode.FindAsync(episodeId);
    }
    public Task<Episode[]> GetEpisodesByAlbumIdAsync(Guid albumId)
    {
        return _dbCtx.Episode.Where(e => e.AlbumId == albumId).OrderBy(e => e.SequenceNumber).ToArrayAsync();
    }
}
