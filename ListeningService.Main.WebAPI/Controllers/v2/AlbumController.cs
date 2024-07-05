using CommonInitializer.ActionDatas;
using ListeningService.Domain;
using ListeningService.Main.WebAPI.Controllers.DTOs;
using Microsoft.AspNetCore.Mvc;
using Zack.ASPNETCore;

namespace ListeningService.Main.WebAPI.Controllers.v2;

[Route("api/v2/[controller]/[action]")]
[ApiController]
[ApiExplorerSettings(GroupName = "v2")] // 版本控制
public class AlbumController : ControllerBase
{
    private readonly IListeningRepository _repository;
    private readonly IMemoryCacheHelper _memoryCache;
    public AlbumController(IListeningRepository repository, IMemoryCacheHelper memoryCache)
    {
        _repository = repository;
        _memoryCache = memoryCache;
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse<AlbumDTO>>> FindById(Guid id)
    {
        var albumDTO = await _memoryCache.GetOrCreateAsync($"AlbumController.FindById.{id}",
            async e => AlbumDTO.Create(await _repository.GetAlbumByIdAsync(id)));

        if (albumDTO == null)
            return NotFound(ApiResponse<AlbumDTO>.NotFound($"The album isn't found with albumId={id}"));

        return ApiResponse<AlbumDTO>.Succeed(albumDTO);
    }

    [HttpGet]
    [Route("{categoryId}")]
    public async Task<ActionResult<ApiResponse<AlbumDTO[]>>> FindAllByCategoryId(Guid categoryId)
    {
        var albumDTOs = await _memoryCache.GetOrCreateAsync($"AlbumController.FindAllByCategoryId.{categoryId}",
            async e => AlbumDTO.Create(await _repository.GetAlbumsByCategoryIdAsync(categoryId)));

        return ApiResponse<AlbumDTO[]>.Succeed(albumDTOs);
    }
}
