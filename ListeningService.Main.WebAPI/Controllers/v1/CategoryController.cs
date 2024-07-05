using ListeningService.Domain;
using ListeningService.Main.WebAPI.Controllers.DTOs;
using Microsoft.AspNetCore.Mvc;
using Zack.ASPNETCore;

namespace ListeningService.Main.WebAPI.Controllers.v1;

[Route("api/[controller]/[action]")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")] // 版本控制
public class CategoryController : ControllerBase
{
    private readonly IListeningRepository _repository;
    private readonly IMemoryCacheHelper _memoryCache;
    public CategoryController(IListeningRepository repository, IMemoryCacheHelper memoryCache)
    {
        _repository = repository;
        _memoryCache = memoryCache;
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<CategoryDTO>> FindById(Guid id)
    {
        var categoryDTO = await _memoryCache.GetOrCreateAsync($"CategoryController.FindById.{id}",
            async e => CategoryDTO.Create(await _repository.GetCategoryByIdAsync(id)));

        if (categoryDTO == null)
            return NotFound($"The category isn't found with categoryId={id}");

        return Ok(categoryDTO);
    }

    [HttpGet]
    public async Task<CategoryDTO[]> FindAll()
    {
        var categoryDTOs = await _memoryCache.GetOrCreateAsync("CategoryController.FindAll",
            async e => CategoryDTO.Create(await _repository.GetCategorysAsync()));

        return categoryDTOs!;
    }
}
