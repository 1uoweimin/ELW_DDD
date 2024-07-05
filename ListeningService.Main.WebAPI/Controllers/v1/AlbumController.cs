using ListeningService.Domain;
using ListeningService.Main.WebAPI.Controllers.DTOs;
using Microsoft.AspNetCore.Mvc;
using Zack.ASPNETCore;

namespace ListeningService.Main.WebAPI.Controllers.v1;

[Route("api/[controller]/[action]")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")] // 版本控制
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
    public async Task<ActionResult<AlbumDTO>> FindById(Guid id)
    {
        var albumDTO = await _memoryCache.GetOrCreateAsync($"AlbumController.FindById.{id}",
            async e => AlbumDTO.Create(await _repository.GetAlbumByIdAsync(id)));

        if (albumDTO == null)
            return NotFound($"The album isn't found with albumId={id}");

        return albumDTO;
    }

    [HttpGet]
    [Route("{categoryId}")]
    public async Task<AlbumDTO[]> FindAllByCategoryId(Guid categoryId)
    {
        var albumDTOs = await _memoryCache.GetOrCreateAsync($"AlbumController.FindAllByCategoryId.{categoryId}",
            async e => AlbumDTO.Create(await _repository.GetAlbumsByCategoryIdAsync(categoryId)));

        return albumDTOs!;
    }
}
