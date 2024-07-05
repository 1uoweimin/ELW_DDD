using ListeningService.Admin.WebAPI.Controllers.v1.Requests;
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

namespace ListeningService.Admin.WebAPI.Controllers.v1;

[Route("api/[controller]/[action]")]
[ApiController]
[UnitOfWork(typeof(ListeningDbContext))]
[Authorize(Roles = "Admin")]
[ApiExplorerSettings(GroupName = "v1")] // 版本控制
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
    public async Task<ActionResult<Guid>> Add(EpisodeAddReq req)
    {
        bool isExist = await _dbCtx.Album.AsNoTracking().AnyAsync(c => c.Id == req.AlbumId);
        if (!isExist)
            return BadRequest($"The albumId={req.AlbumId} don't exist");

        Guid episodeId = Guid.NewGuid();
        if (!req.AudioUrl.ToString().EndsWith("m4a", StringComparison.OrdinalIgnoreCase)) // 非m4a格式的文件
        {
            var episodeInfo = new EncodingEpisodeInfo(episodeId, req.Name, req.AlbumId, req.DurationInSecond, req.Subtitle, EEInofStatus.Created);
            await _encodingEpisode.AddEncodingEpisodeAsync(episodeInfo);

            // 通知转码
            _eventBus.Publish("MediaEncoding.Created", new { MediaId = episodeId, MediaUrl = req.AudioUrl, OutputFormat = "m4a", SourceSystem = "ListeningServcie" });
            _logger.LogInformation($"（非m4a格式的文件）通知转码，发送集成事件【MediaEncoding.Created】，MediaId={episodeId}");
        }
        else // m4a格式的文件
        {
            var episode = await _domainService.AddEpisodeAsync(episodeId, req.AlbumId, req.Name, req.AudioUrl, req.DurationInSecond, req.Subtitle);
            await _dbCtx.AddAsync(episode);
            _logger.LogInformation($"（m4a格式的文件）添加 Episode {episode}");
        }
        return Ok(episodeId);
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult> DeleteById(Guid id)
    {
        var episode = await _repository.GetEpisodeByIdAsync(id);
        if (episode == null)
            return NotFound($"The episode isn't found with episodeId={id}");
        episode.SoftDelete();
        return Ok();
    }

    [HttpPut]
    public async Task<ActionResult> Update(EpisodeUpdateReq req)
    {
        var episode = await _repository.GetEpisodeByIdAsync(req.Id);
        if (episode == null)
            return NotFound($"The episode isn't found with episodeId={req.Id}");
        episode.ChangeName(req.Name).ChangeSubtitle(req.Subtitle);
        return Ok();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<Episode>> FindById(Guid id)
    {
        var episode = await _repository.GetEpisodeByIdAsync(id);
        if (episode == null)
            return NotFound($"The episode isn't found with episodeId={id}");
        return episode;
    }

    [HttpGet]
    [Route("{albumId}")]
    public async Task<ActionResult<Episode[]>> FindAll(Guid albumId)
    {
        var episodes = await _repository.GetEpisodesByAlbumIdAsync(albumId);
        return episodes;
    }

    [HttpGet]
    [Route("{albumId}")]
    public async Task<ActionResult<EncodingEpisodeInfo[]>> FindEncodingEpisodesByAlbumId(Guid albumId)
    {
        var idsOfAlbum = await _encodingEpisode.GetEncodingEpisodeIdsOfAlbumAsync(albumId);

        List<EncodingEpisodeInfo> episodeInfos = new List<EncodingEpisodeInfo>();
        foreach (var id in idsOfAlbum)
        {
            var episodeInfo = await _encodingEpisode.GetEncodingEpisodeAsync(id);
            if (!episodeInfo.Status.Equals("Completed"))
                episodeInfos.Add(episodeInfo);
        }

        return episodeInfos.ToArray();
    }

    [HttpPost]
    public async Task<ActionResult> Sort(EpisodesSortReq req)
    {
        bool isExist = await _dbCtx.Album.AsNoTracking().AnyAsync(a => a.Id == req.AlbumId);
        if (!isExist)
            return BadRequest($"The albumId={req.AlbumId} don't exist");

        await _domainService.SortEpisodesAsync(req.AlbumId, req.Ids);
        return Ok();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult> Show(Guid id)
    {
        var episdoe = await _repository.GetEpisodeByIdAsync(id);
        if (episdoe == null)
            return NotFound($"The episode isn't found with episode={id}");
        episdoe.Show();
        return Ok();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult> Hide(Guid id)
    {
        var episdoe = await _repository.GetEpisodeByIdAsync(id);
        if (episdoe == null)
            return NotFound($"The episode isn't found with episode={id}");
        episdoe.Hide();
        return Ok();
    }
}