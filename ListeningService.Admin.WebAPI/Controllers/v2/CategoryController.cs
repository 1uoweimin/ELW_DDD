using CommonInitializer.ActionDatas;
using ListeningService.Admin.WebAPI.Controllers.v2.Requests;
using ListeningService.Admin.WebAPI.Controllers.v2.Responses;
using ListeningService.Domain;
using ListeningService.Domain.Entities;
using ListeningService.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zack.ASPNETCore;

namespace ListeningService.Admin.WebAPI.Controllers.v2;

[Route("api/v2/[controller]/[action]")]
[ApiController]
[UnitOfWork(typeof(ListeningDbContext))]
[Authorize(Roles = "Admin")]
[ApiExplorerSettings(GroupName = "v2")] // 版本控制
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
    public async Task<ActionResult<ApiResponse<IdRsp>>> Add(ApiRequest<CategoryAddReq> req)
    {
        var category = await _domainService.AddCategoryAsync(req.ReqData.Name, req.ReqData.Url);
        _dbCtx.Add(category);
        return ApiResponse<IdRsp>.Succeed(new(category.Id));
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteById(Guid id)
    {
        var category = await _repository.GetCategoryByIdAsync(id);
        if (category == null)
            return NotFound(ApiResponse<string>.NotFound($"The category isn't found with categoryId={id}"));
        category.SoftDelete();
        return ApiResponse<string>.Succeed();
    }

    [HttpPut]
    public async Task<ActionResult<ApiResponse<string>>> Update(ApiRequest<CategoryUpdateReq> req)
    {
        var category = await _repository.GetCategoryByIdAsync(req.ReqData.Id);
        if (category == null)
            return NotFound(ApiResponse<string>.NotFound($"The category isn't found with categoryId={req.ReqData.Id}"));
        category.ChangeName(req.ReqData.Name).ChangeCoverUrl(req.ReqData.Url);
        return ApiResponse<string>.Succeed();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse<Category>>> FindById(Guid id)
    {
        var category = await _repository.GetCategoryByIdAsync(id);
        if (category == null)
            return NotFound(ApiResponse<Category>.NotFound($"The category isn't found with categoryId={id}"));
        return ApiResponse<Category>.Succeed(category);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<Category[]>>> FindAll()
    {
        var categories = await _repository.GetCategorysAsync();
        return ApiResponse<Category[]>.Succeed(categories);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> Sort(CategoriesSortReq req)
    {
        await _domainService.SortCategoriesAsync(req.Ids);
        return ApiResponse<string>.Succeed();
    }
}