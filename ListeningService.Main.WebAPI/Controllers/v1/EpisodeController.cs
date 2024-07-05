using ListeningService.Domain;
using ListeningService.Main.WebAPI.Controllers.DTOs;
using Microsoft.AspNetCore.Mvc;
using Zack.ASPNETCore;

namespace ListeningService.Main.WebAPI.Controllers.v1;

[Route("api/[controller]/[action]")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")] // 版本控制
public class EpisodeController : ControllerBase
{
    private readonly IListeningRepository _repository;
    private readonly IMemoryCacheHelper _memoryCache;
    public EpisodeController(IListeningRepository repository, IMemoryCacheHelper memoryCache)
    {
        _repository = repository;
        _memoryCache = memoryCache;
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<EpisodeDTO>> FindById(Guid id)
    {
        var episodeDTO = await _memoryCache.GetOrCreateAsync($"EpisodeController.EpisodeDTO.{id}",
           async e => EpisodeDTO.Create(await _repository.GetEpisodeByIdAsync(id), true));

        if (episodeDTO == null)
            return NotFound($"The episode isn't found with episodeId={id}");

        return episodeDTO;
    }

    [HttpGet]
    [Route("{albumId}")]
    public async Task<EpisodeDTO[]> FindAllByAlbumId(Guid albumId)
    {
        var episodeDTOs = await _memoryCache.GetOrCreateAsync($"EpisodeController.FindAllByAlbumId.{albumId}",
            async e => EpisodeDTO.Create(await _repository.GetEpisodesByAlbumIdAsync(albumId), false));

        return episodeDTOs!;
    }
}
