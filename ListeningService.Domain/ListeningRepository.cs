using ListeningService.Domain.Entities;

namespace ListeningService.Domain;

/// <summary>
/// 听力仓储接口
/// </summary>
public interface IListeningRepository
{
    Task<int> GetMaxSequenceOfCategoryAsync();
    Task<Category?> GetCategoryByIdAsync(Guid categoryId);
    Task<Category[]> GetCategorysAsync();

    Task<int> GetMaxSequenceOfAlbumAsync(Guid categoryId);
    Task<Album?> GetAlbumByIdAsync(Guid albumId);
    Task<Album[]> GetAlbumsByCategoryIdAsync(Guid categoryId);

    Task<int> GetMaxSequenceOfEpisodeAsync(Guid albumId);
    Task<Episode?> GetEpisodeByIdAsync(Guid episodeId);
    Task<Episode[]> GetEpisodesByAlbumIdAsync(Guid albumId);
}