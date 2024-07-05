using CommonInitializer.ActionDatas;
using ListeningService.Domain;
using ListeningService.Main.WebAPI.Controllers.DTOs;
using Microsoft.AspNetCore.Mvc;
using Zack.ASPNETCore;

namespace ListeningService.Main.WebAPI.Controllers.v2;

[Route("api/v2/[controller]/[action]")]
[ApiController]
[ApiExplorerSettings(GroupName = "v2")] // 版本控制
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
    public async Task<ActionResult<ApiResponse<EpisodeDTO>>> FindById(Guid id)
    {
        var episodeDTO = await _memoryCache.GetOrCreateAsync($"EpisodeController.EpisodeDTO.{id}",
           async e => EpisodeDTO.Create(await _repository.GetEpisodeByIdAsync(id), true));

        if (episodeDTO == null)
            return NotFound(ApiResponse<EpisodeDTO>.NotFound($"The episode isn't found with episodeId={id}"));

        return ApiResponse<EpisodeDTO>.Succeed(episodeDTO);
    }

    [HttpGet]
    [Route("{albumId}")]
    public async Task<ApiResponse<EpisodeDTO[]>> FindAllByAlbumId(Guid albumId)
    {
        var episodeDTOs = await _memoryCache.GetOrCreateAsync($"EpisodeController.FindAllByAlbumId.{albumId}",
            async e => EpisodeDTO.Create(await _repository.GetEpisodesByAlbumIdAsync(albumId), false));

        return ApiResponse<EpisodeDTO[]>.Succeed(episodeDTOs);
    }
}
