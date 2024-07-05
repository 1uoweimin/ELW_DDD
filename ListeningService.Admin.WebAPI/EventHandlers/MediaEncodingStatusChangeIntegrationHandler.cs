using ListeningService.Admin.WebAPI.EncodingEpisodeHelper;
using ListeningService.Admin.WebAPI.EncodingEpisodeTool;
using ListeningService.Admin.WebAPI.Hubs;
using ListeningService.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Zack.EventBus;

namespace ListeningService.Domain.EventHandlers;

/// <summary>
/// 监听转码服务发送的集成事件消息，把状态通过SignalR推送给客户端（显示转码状态）
/// </summary>
[EventName("MediaEncoding.Started")]
[EventName("MediaEncoding.Completed")]
[EventName("MediaEncoding.Failed")]
[EventName("MediaEncoding.Duplicated")]
internal class MediaEncodingStatusChangeIntegrationHandler : DynamicIntegrationEventHandler
{
    private readonly ListeningDbContext _dbCtx;
    private readonly ListeningDomainService _domainService;
    private readonly IEncodingEpisode _encodingEpisode;
    private readonly IHubContext<EpisodeEncodingStatusHub> _hubContext;
    private readonly ILogger<MediaEncodingStatusChangeIntegrationHandler> _logger;
    public MediaEncodingStatusChangeIntegrationHandler(ListeningDbContext dbCtx, ListeningDomainService domainService, IEncodingEpisode encodingEpisode, IHubContext<EpisodeEncodingStatusHub> hubContext, ILogger<MediaEncodingStatusChangeIntegrationHandler> logger)
    {
        _dbCtx = dbCtx;
        _domainService = domainService;
        _encodingEpisode = encodingEpisode;
        _hubContext = hubContext;
        _logger = logger;
    }

    public override async Task HandleDynamic(string eventName, dynamic eventData)
    {
        //忽略不是 ListeningServcie 系统的转码信息
        if (eventData.SourceSystem != "ListeningServcie") return;

        Guid episodeId = Guid.Parse(eventData.Id);
        switch (eventName)
        {
            //Todo：_hubContext.Clients.All.SendAsync()这样会把消息发送给所有打开这个界面的人，应该用connectionId、userId等进行过滤。

            case "MediaEncoding.Started":
                _logger.LogInformation($"监听处理集成事件 MediaEncoding.Started");

                await _encodingEpisode.UpdateEncodingEpisodeStatusAsync(episodeId, EEInofStatus.Started);
                await _hubContext.Clients.All.SendAsync("OnMediaEncodingStarted", episodeId);
                break;

            case "MediaEncoding.Completed":
                _logger.LogInformation($"监听处理集成事件 MediaEncoding.Completed");

                //转码完成，则从Redis中把暂存的Episode信息取出来，然后正式地插入Episode表中
                await _encodingEpisode.UpdateEncodingEpisodeStatusAsync(episodeId, EEInofStatus.Completed);

                var episodeInfo = await _encodingEpisode.GetEncodingEpisodeAsync(episodeId);
                var episode = await _domainService.AddEpisodeAsync(episodeId, episodeInfo.AlbumId, episodeInfo.Name, new Uri(eventData.OutputUrl), episodeInfo.DurationInSecond, episodeInfo.Subtitle);
                await _dbCtx.AddAsync(episode);
                await _dbCtx.SaveChangesAsync();

                await _hubContext.Clients.All.SendAsync("OnMediaEncodingCompleted", episodeId);
                break;

            case "MediaEncoding.Failed":
                _logger.LogInformation($"监听处理集成事件 MediaEncoding.Failed");

                await _encodingEpisode.UpdateEncodingEpisodeStatusAsync(episodeId, EEInofStatus.Failed);
                await _hubContext.Clients.All.SendAsync("OnMediaEncodingFailed", episodeId);
                break;

            case "MediaEncoding.Duplicated":
                _logger.LogInformation($"监听处理集成事件 MediaEncoding.Duplicated");

                await _encodingEpisode.UpdateEncodingEpisodeStatusAsync(episodeId, EEInofStatus.Completed);
                await _hubContext.Clients.All.SendAsync("OnMediaEncodingCompleted", episodeId);
                break;
        }
    }
}
