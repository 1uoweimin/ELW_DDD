using SearchService.DomainService;
using SearchService.DomainService.Entities;
using Zack.EventBus;

namespace SearchService.WebAPI.EventHandlers;

/// <summary>
/// 监听音频的创建和更新的处理器（把音频存储起来或者已经存储的音频进行更新）
/// </summary>
[EventName("ListeningService.Episode.Create")]
[EventName("ListeningService.Episode.Update")]
public class ListeningEpisodeUpsertEventHandler : DynamicIntegrationEventHandler
{
    private readonly ISearchRepository repository;
    public ListeningEpisodeUpsertEventHandler(ISearchRepository repository)
    {
        this.repository = repository;
    }

    public override Task HandleDynamic(string eventName, dynamic eventData)
    {
        Guid id = Guid.Parse(eventData.Id);
        string cnName = eventData.Name.Chinese;
        string engName = eventData.Name.English;
        Guid albumId = Guid.Parse(eventData.AlbumId);

        List<string> sentenceList = new List<string>();
        foreach (var sentence in eventData.Sentences)
            if (sentence != null && sentence!.Content != null)
                sentenceList.Add(sentence!.Content);
        string plainSentences = string.Join("\r\n", sentenceList);

        Episode episode = new Episode(id, cnName, engName, plainSentences, albumId);
        return repository.UpsertEpisodeAsync(episode);
    }
}
