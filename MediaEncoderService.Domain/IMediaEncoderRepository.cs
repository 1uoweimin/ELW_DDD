using MediaEncoderService.Domain.Entities;

namespace MediaEncoderService.Domain;

/// <summary>
/// 转码仓储接口
/// </summary>
public interface IMediaEncoderRepository
{
    Task<EncodingItem?> FindCompletedOneAsync(long fileSizeInBytes, string fileSHA256Hash);

    Task<EncodingItem[]> FindAsync(ItemStatus itemStatus);
}
