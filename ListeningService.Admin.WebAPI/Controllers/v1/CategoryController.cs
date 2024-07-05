using ListeningService.Admin.WebAPI.Controllers.v1.Requests;
using ListeningService.Domain;
using ListeningService.Domain.Entities;
using ListeningService.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zack.ASPNETCore;

namespace ListeningService.Admin.WebAPI.Controllers.v1;

[Route("api/[controller]/[action]")]
[ApiController]
[UnitOfWork(typeof(ListeningDbContext))]
[Authorize(Roles = "Admin")]
[ApiExplorerSettings(GroupName = "v1")] // 版本控制
public class CategoryController : ControllerBase
{
    private readonly ListeningDomainService _domainService;
    private readonly IListeningRepository _repository;
    private readonly ListeningDbContext _dbCtx;
    public CategoryController(ListeningDomainService domainService, IListeningRepository repository, ListeningDbContext dbCtx)
    {
        _domainService = domainService;
        _repository = repository;
        _dbCtx = dbCtx;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Add(CategoryAddReq req)
    {
        var category = await _domainService.AddCategoryAsync(req.Name, req.Url);
        _dbCtx.Add(category);
        return Ok(category.Id);
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult> DeleteById(Guid id)
    {
        var category = await _repository.GetCategoryByIdAsync(id);
        if (category == null)
            return NotFound($"The category isn't found with categoryId={id}");
        category.SoftDelete();
        return Ok();
    }

    [HttpPut]
    public async Task<ActionResult> Update(CategoryUpdateReq req)
    {
        var category = await _repository.GetCategoryByIdAsync(req.Id);
        if (category == null)
            return NotFound($"The category isn't found with categoryId={req.Id}");
        category.ChangeName(req.Name).ChangeCoverUrl(req.Url);
        return Ok();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<Category>> FindById(Guid id)
    {
        var category = await _repository.GetCategoryByIdAsync(id);
        if (category == null)
            return NotFound($"The category isn't found with categoryId={id}");
        return Ok(category);
    }

    [HttpGet]
    public async Task<ActionResult<Category[]>> FindAll()
    {
        var categories = await _repository.GetCategorysAsync();
        return categories;
    }

    [HttpPost]
    public async Task<ActionResult> Sort(CategoriesSortReq req)
    {
        await _domainService.SortCategoriesAsync(req.Ids);
        return Ok();
    }
}