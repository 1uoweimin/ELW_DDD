using CommonInitializer.ActionDatas;
using ListeningService.Admin.WebAPI.Controllers.v1.Requests;
using ListeningService.Admin.WebAPI.Controllers.v2.Responses;
using ListeningService.Admin.WebAPI.EncodingEpisodeHelper;
using ListeningService.Admin.WebAPI.EncodingEpisodeTool;
using ListeningService.Domain;
using ListeningService.Domain.Entities;
using ListeningService.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zack.ASPNETCore;
using Zack.EventBus;

namespace ListeningService.Admin.WebAPI.Controllers.v2;

[Route("api/v2/[controller]/[action]")]
[ApiController]
[UnitOfWork(typeof(ListeningDbContext))]
[Authorize(Roles = "Admin")]
[ApiExplorerSettings(GroupName = "v2")] // 版本控制
public class EpisodeController : ControllerBase
{
    private readonly ListeningDomainService _domainService;
    private readonly IListeningRepository _repository;
    private readonly ListeningDbContext _dbCtx;
    private readonly IEventBus _eventBus;
    private readonly IEncodingEpisode _encodingEpisode;
    private readonly ILogger<EpisodeController> _logger;
    public EpisodeController(ListeningDomainService domainService, IListeningRepository repository, ListeningDbContext dbCtx, IEventBus eventBus, IEncodingEpisode encodingEpisode, ILogger<EpisodeController> logger)
    {
        _domainService = domainService;
        _repository = repository;
        _dbCtx = dbCtx;
        _eventBus = eventBus;
        _encodingEpisode = encodingEpisode;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<IdRsp>>> Add(ApiRequest<EpisodeAddReq> req)
    {
        bool isExist = await _dbCtx.Album.AsNoTracking().AnyAsync(c => c.Id == req.ReqData.AlbumId);
        if (!isExist)
            return NotFound(ApiResponse<IdRsp>.NotFound($"The albumId={req.ReqData.AlbumId} don't exist"));

        Guid episodeId = Guid.NewGuid();
        if (!req.ReqData.AudioUrl.ToString().EndsWith("m4a", StringComparison.OrdinalIgnoreCase)) // 非m4a格式的文件
        {
            var episodeInfo = new EncodingEpisodeInfo(episodeId, req.ReqData.Name, req.ReqData.AlbumId, req.ReqData.DurationInSecond, req.ReqData.Subtitle, EEInofStatus.Created);
            await _encodingEpisode.AddEncodingEpisodeAsync(episodeInfo);

            // 通知转码
            _eventBus.Publish("MediaEncoding.Created", new { MediaId = episodeId, MediaUrl = req.ReqData.AudioUrl, OutputFormat = "m4a", SourceSystem = "ListeningServcie" });
            _logger.LogInformation($"（非m4a格式的文件）通知转码，发送集成事件【MediaEncoding.Created】，MediaId={episodeId}");
        }
        else // m4a格式的文件
        {
            var episode = await _domainService.AddEpisodeAsync(episodeId, req.ReqData.AlbumId, req.ReqData.Name, req.ReqData.AudioUrl, req.ReqData.DurationInSecond, req.ReqData.Subtitle);
            await _dbCtx.AddAsync(episode);
            _logger.LogInformation($"（m4a格式的文件）添加 Episode {episode}");
        }
        return ApiResponse<IdRsp>.Succeed(new(episodeId));
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteById(Guid id)
    {
        var episode = await _repository.GetEpisodeByIdAsync(id);
        if (episode == null)
            return NotFound(ApiResponse<string>.NotFound($"The episode isn't found with episodeId={id}"));
        episode.SoftDelete();
        return ApiResponse<string>.Succeed();
    }

    [HttpPut]
    public async Task<ActionResult<ApiResponse<string>>> Update(ApiRequest<EpisodeUpdateReq> req)
    {
        var episode = await _repository.GetEpisodeByIdAsync(req.ReqData.Id);
        if (episode == null)
            return NotFound(ApiResponse<string>.NotFound($"The episode isn't found with episodeId={req.ReqData.Id}"));
        episode.ChangeName(req.ReqData.Name).ChangeSubtitle(req.ReqData.Subtitle);
        return ApiResponse<string>.Succeed();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse<Episode>>> FindById(Guid id)
    {
        var episode = await _repository.GetEpisodeByIdAsync(id);
        if (episode == null)
            return NotFound(ApiResponse<Episode>.NotFound($"The episode isn't found with episodeId={id}"));
        return ApiResponse<Episode>.Succeed(episode);
    }

    [HttpGet]
    [Route("{albumId}")]
    public async Task<ActionResult<ApiResponse<Episode[]>>> FindAll(Guid albumId)
    {
        var episodes = await _repository.GetEpisodesByAlbumIdAsync(albumId);
        return ApiResponse<Episode[]>.Succeed(episodes);
    }

    [HttpGet]
    [Route("{albumId}")]
    public async Task<ActionResult<ApiResponse<EncodingEpisodeInfo[]>>> FindEncodingEpisodesByAlbumId(Guid albumId)
    {
        var idsOfAlbum = await _encodingEpisode.GetEncodingEpisodeIdsOfAlbumAsync(albumId);

        List<EncodingEpisodeInfo> episodeInfos = new List<EncodingEpisodeInfo>();
        foreach (var id in idsOfAlbum)
        {
            var episodeInfo = await _encodingEpisode.GetEncodingEpisodeAsync(id);
            if (!episodeInfo.Status.Equals("Completed"))
                episodeInfos.Add(episodeInfo);
        }

        return ApiResponse<EncodingEpisodeInfo[]>.Succeed(episodeInfos.ToArray());
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> Sort(ApiRequest<EpisodesSortReq> req)
    {
        bool isExist = await _dbCtx.Album.AsNoTracking().AnyAsync(a => a.Id == req.ReqData.AlbumId);
        if (!isExist)
            return NotFound(ApiResponse<string>.NotFound($"The albumId={req.ReqData.AlbumId} don't exist"));

        await _domainService.SortEpisodesAsync(req.ReqData.AlbumId, req.ReqData.Ids);
        return ApiResponse<string>.Succeed();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Show(Guid id)
    {
        var episdoe = await _repository.GetEpisodeByIdAsync(id);
        if (episdoe == null)
            return NotFound(ApiResponse<string>.NotFound($"The episode isn't found with episode={id}"));
        episdoe.Show();
        return ApiResponse<string>.Succeed();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Hide(Guid id)
    {
        var episdoe = await _repository.GetEpisodeByIdAsync(id);
        if (episdoe == null)
            return NotFound(ApiResponse<string>.NotFound($"The episode isn't found with episode={id}"));
        episdoe.Hide();
        return ApiResponse<string>.Succeed();
    }
}