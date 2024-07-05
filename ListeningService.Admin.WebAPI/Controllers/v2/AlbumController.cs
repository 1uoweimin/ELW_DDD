using CommonInitializer.ActionDatas;
using ListeningService.Admin.WebAPI.Controllers.v2.Requests;
using ListeningService.Admin.WebAPI.Controllers.v2.Responses;
using ListeningService.Domain;
using ListeningService.Domain.Entities;
using ListeningService.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zack.ASPNETCore;

namespace ListeningService.Admin.WebAPI.Controllers.v2;

[Route("api/v2/[controller]/[action]")]
[ApiController]
[UnitOfWork(typeof(ListeningDbContext))]
[Authorize(Roles = "Admin")]
[ApiExplorerSettings(GroupName = "v2")] // 版本控制
public class AlbumController : ControllerBase
{
    private readonly ListeningDomainService _domainService;
    private readonly IListeningRepository _repository;
    private readonly ListeningDbContext _dbCtx;
    public AlbumController(ListeningDomainService domainService, IListeningRepository repository, ListeningDbContext dbCtx)
    {
        _domainService = domainService;
        _repository = repository;
        _dbCtx = dbCtx;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<IdRsp>>> Add(ApiRequest<AlbumAddReq> req)
    {
        bool isExist = await _dbCtx.Category.AsNoTracking().AnyAsync(c => c.Id == req.ReqData.CategoryId);
        if (!isExist)
            return NotFound(ApiResponse<IdRsp>.NotFound($"The categoryId={req.ReqData.CategoryId} don't exist"));

        var album = await _domainService.AddAlbumAsync(req.ReqData.CategoryId, req.ReqData.Name);
        await _dbCtx.AddAsync(album);
        return ApiResponse<IdRsp>.Succeed(new(album.Id));
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteById(Guid id)
    {
        var album = await _repository.GetAlbumByIdAsync(id);
        if (album == null)
            return NotFound(ApiResponse<string>.NotFound($"The album isn't found with albumId={id}"));
        album.SoftDelete();
        return ApiResponse<string>.Succeed();
    }

    [HttpPut]
    public async Task<ActionResult<ApiResponse<string>>> Update(ApiRequest<AlbumUpdateReq> req)
    {
        var album = await _repository.GetAlbumByIdAsync(req.ReqData.Id);
        if (album == null)
            return NotFound(ApiResponse<string>.NotFound($"The album isn't found with albumId={req.ReqData.Id}"));
        album.ChangeName(req.ReqData.Name);
        return ApiResponse<string>.Succeed();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse<Album>>> FindById(Guid id)
    {
        var album = await _repository.GetAlbumByIdAsync(id);
        if (album == null)
            return NotFound(ApiResponse<Album>.NotFound($"The album isn't found with albumId={id}"));
        return ApiResponse<Album>.Succeed(album);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse<Album[]>>> FindAllByCategoryId(Guid id)
    {
        var albums = await _repository.GetAlbumsByCategoryIdAsync(id);
        return ApiResponse<Album[]>.Succeed(albums);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> Sort(ApiRequest<AlbumsSortReq> req)
    {
        await _domainService.SortAlbumsAsync(req.ReqData.CategoryId, req.ReqData.Ids);
        return ApiResponse<string>.Succeed();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Show(Guid id)
    {
        var album = await _repository.GetAlbumByIdAsync(id);
        if (album == null)
            return NotFound(ApiResponse<string>.NotFound($"The album isn't found with albumId={id}"));
        album.Show();
        return ApiResponse<string>.Succeed();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Hide(Guid id)
    {
        var album = await _repository.GetAlbumByIdAsync(id);
        if (album == null)
            return NotFound(ApiResponse<string>.NotFound($"The album isn't found with albumId={id}"));
        album.Hide();
        return ApiResponse<string>.Succeed();
    }
}