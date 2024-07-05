using FileService.Domain;
using FileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileService.Infrastructure;

/// <summary>
/// 仓储实现
/// </summary>
public class FSRepository : IFSRepository
{
    private readonly FSDbContext _dbContext;
    public FSRepository(FSDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<UploadedItem?> FindFileAsync(long fileSizeInBytes, string fileSHA256Hash)
    {
        return _dbContext.UploadedItems.FirstOrDefaultAsync(u => u.FileSizeInBytes == fileSizeInBytes && u.FileSHA256Hash == fileSHA256Hash);
    }
}