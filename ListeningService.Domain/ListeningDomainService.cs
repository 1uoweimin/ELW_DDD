using ListeningService.Domain.Entities;
using ListeningService.Domain.ValueObjects;
using Zack.DomainCommons.Models;

namespace ListeningService.Domain;

/// <summary>
/// 听力领域服务：领域服务只有抽象的业务逻辑。
/// </summary>
public class ListeningDomainService
{
    private readonly IListeningRepository _repository;
    public ListeningDomainService(IListeningRepository listeningRepository)
    {
        _repository = listeningRepository;
    }

    public async Task<Category> AddCategoryAsync(MultilingualString name, Uri coverUrl)
    {
        int maxSequence = await _repository.GetMaxSequenceOfCategoryAsync();
        Category category = new Category(maxSequence + 1, name, coverUrl);
        return category;
    }
    public async Task SortCategoriesAsync(Guid[] sortCategoryIds)
    {
        var categorys = await _repository.GetCategorysAsync();
        var categoryIds = categorys.Select(c => c.Id);
        if (!categoryIds.SequenceIgnoredEqual(sortCategoryIds))
            throw new ApplicationException("提交的待排序Id中必须是所有的分类Id");

        int sequenceNum = 1;
        foreach (var categoryId in sortCategoryIds)
        {
            var category = categorys.First(c => c.Id == categoryId);
            category.ChangeSequenceNumber(sequenceNum);
            sequenceNum++;
        }
    }

    public async Task<Album> AddAlbumAsync(Guid categoryId, MultilingualString name)
    {
        int maxSequence = await _repository.GetMaxSequenceOfAlbumAsync(categoryId);
        Album album = new Album(maxSequence + 1, name, categoryId);
        return album;
    }
    public async Task SortAlbumsAsync(Guid categoryId, Guid[] sortAlbumIds)
    {
        var albums = await _repository.GetAlbumsByCategoryIdAsync(categoryId);
        var albumIds = albums.Select(c => c.Id);
        if (!albumIds.SequenceIgnoredEqual(sortAlbumIds))
            throw new ApplicationException("提交的待排序Id中必须是所有的分类Id");

        int sequenceNum = 1;
        foreach (var albumId in sortAlbumIds)
        {
            var album = albums.First(c => c.Id == albumId);
            album.ChangeSequenceNumber(sequenceNum);
            sequenceNum++;
        }
    }

    public async Task<Episode> AddEpisodeAsync(Guid Id, Guid albumId, MultilingualString name, Uri audioUrl, double durationInSeconds, Subtitle subtitle)
    {
        int maxSequence = await _repository.GetMaxSequenceOfEpisodeAsync(albumId);
        Episode episode = new Episode(Id, maxSequence + 1, name, albumId, audioUrl, durationInSeconds, subtitle);
        return episode;
    }
    public async Task SortEpisodesAsync(Guid albumId, Guid[] sortEpisodeIds)
    {
        var episodes = await _repository.GetEpisodesByAlbumIdAsync(albumId);
        var episodeIds = episodes.Select(c => c.Id);
        if (!episodeIds.SequenceIgnoredEqual(sortEpisodeIds))
            throw new ApplicationException("提交的待排序Id中必须是所有的分类Id");

        int sequenceNum = 1;
        foreach (var episodeId in sortEpisodeIds)
        {
            var episode = episodes.First(c => c.Id == episodeId);
            episode.ChangeSequenceNumber(sequenceNum);
            sequenceNum++;
        }
    }
}
