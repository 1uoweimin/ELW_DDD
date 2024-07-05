using MediaEncoderService.Domain.Entities;
using MediaEncoderService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Zack.EventBus;

namespace MediaEncoderService.WebAPI.EventHandlers;

/// <summary>
/// 其他服务需要转码的时候，会发布一个名字为 MediaEncoding.Create 的集成事件。
/// 从事件携带的数据中解析待转码的文件路径等信息，然后把数据以 EncodingItem 对象的形式插入数据库即可完成转码任务的排队。
/// </summary>
[EventName("MediaEncoding.Created")]
public class MediaEncodingCreatedEventHandler : DynamicIntegrationEventHandler
{
    private readonly MediaEncoderDbContext _dbCtx;
    private readonly ILogger<MediaEncodingCreatedEventHandler> _logger;
    public MediaEncodingCreatedEventHandler(MediaEncoderDbContext dbCtx, ILogger<MediaEncodingCreatedEventHandler> logger)
    {
        _dbCtx = dbCtx;
        _logger = logger;
    }

    public override async Task HandleDynamic(string eventName, dynamic eventData)
    {
        _logger.LogInformation("监听处理集成事件MediaEncoding.Created");

        Guid mediaId = Guid.Parse(eventData.MediaId);
        Uri mediaUrl = new Uri(eventData.MediaUrl);
        string outputFormat = eventData.OutputFormat;
        string sourceSystem = eventData.SourceSystem;
        string fileName = WebUtility.UrlDecode(mediaUrl.Segments.Last());

        //RabbitMQ有重复发送一条消息的可能，所以要确保幂等性（如果检查到这个任务已经被插入数据库了，就返回）
        var exist = await _dbCtx.EncodingItems.AnyAsync(e => e.SourceUrl == mediaUrl && e.OutputFormat == outputFormat);
        if (exist)
        {
            _logger.LogInformation($"待转码的记录 mediaUrl:{mediaUrl},outputFormat:{outputFormat} 已经存在,直接返回。");
            return;
        }

        //把任务插入数据库，也可以看作是一种事件，不一定非要放到MQ中才叫事件
        //没有通过领域事件执行，因为如果一下子来很多任务，领域事件就会并发转码，而这种方式则会一个个的转码
        EncodingItem encodingItem = new EncodingItem(mediaId, fileName, sourceSystem, mediaUrl, outputFormat);
        _logger.LogInformation($"添加EncodingItem记录：{encodingItem}");
        _dbCtx.EncodingItems.Add(encodingItem);
        await _dbCtx.SaveChangesAsync();
    }
}
