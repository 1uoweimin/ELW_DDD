using SearchService.DomainService;
using Zack.EventBus;

namespace SearchService.WebAPI.EventHandlers;

/// <summary>
/// 监听音频的删除和隐藏的处理器（把已经存储的音频删掉）
/// </summary>
[EventName("ListeningService.Episode.SoftDelete")]
[EventName("ListeningService.Episode.Hide")]
public class ListeningEpisodeDeletedEventHandler : DynamicIntegrationEventHandler
{
    private readonly ISearchRepository repository;
    public ListeningEpisodeDeletedEventHandler(ISearchRepository repository)
    {
        this.repository = repository;
    }

    public override Task HandleDynamic(string eventName, dynamic eventData)
    {
        Guid id = Guid.Parse(eventData.Id);
        return repository.DeleteEpisodeAsync(id);
    }
}