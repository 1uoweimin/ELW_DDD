using MediaEncoderService.Domain;
using MediaEncoderService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaEncoderService.Infrastructure;

public class MediaEncoderRepositroy : IMediaEncoderRepository
{
    private readonly MediaEncoderDbContext _dbCtx;
    public MediaEncoderRepositroy(MediaEncoderDbContext dbCtx)
    {
        _dbCtx = dbCtx;
    }

    public Task<EncodingItem?> FindCompletedOneAsync(long fileSizeInBytes, string fileSHA256Hash)
    {
        return _dbCtx.EncodingItems.FirstOrDefaultAsync(e => e.FileSizeInBytes == fileSizeInBytes
             && e.FileSHA256Hash == fileSHA256Hash && e.Status == ItemStatus.Completed);
    }

    public async Task<EncodingItem[]> FindAsync(ItemStatus itemStatus)
    {
        return await _dbCtx.EncodingItems.Where(e => e.Status == itemStatus).ToArrayAsync();
    }
}
