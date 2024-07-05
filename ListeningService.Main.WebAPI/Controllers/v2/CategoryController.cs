using CommonInitializer.ActionDatas;
using ListeningService.Domain;
using ListeningService.Main.WebAPI.Controllers.DTOs;
using Microsoft.AspNetCore.Mvc;
using Zack.ASPNETCore;

namespace ListeningService.Main.WebAPI.Controllers.v2;

[Route("api/v2/[controller]/[action]")]
[ApiController]
[ApiExplorerSettings(GroupName = "v2")] // 版本控制
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
    public async Task<ActionResult<ApiResponse<CategoryDTO>>> FindById(Guid id)
    {
        var categoryDTO = await _memoryCache.GetOrCreateAsync($"CategoryController.FindById.{id}",
            async e => CategoryDTO.Create(await _repository.GetCategoryByIdAsync(id)));

        if (categoryDTO == null)
            return NotFound(ApiResponse<CategoryDTO>.NotFound($"The category isn't found with categoryId={id}"));

        return ApiResponse<CategoryDTO>.Succeed(categoryDTO);
    }

    [HttpGet]
    public async Task<ApiResponse<CategoryDTO[]>> FindAll()
    {
        var categoryDTOs = await _memoryCache.GetOrCreateAsync("CategoryController.FindAll",
            async e => CategoryDTO.Create(await _repository.GetCategorysAsync()));

        return ApiResponse<CategoryDTO[]>.Succeed(categoryDTOs);
    }
}
