using ListeningService.Admin.WebAPI.Controllers.v1.Requests;
using ListeningService.Domain;
using ListeningService.Domain.Entities;
using ListeningService.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zack.ASPNETCore;

namespace ListeningService.Admin.WebAPI.Controllers.v1;

[Route("api/[controller]/[action]")]
[ApiController]
[UnitOfWork(typeof(ListeningDbContext))]
[Authorize(Roles = "Admin")]
[ApiExplorerSettings(GroupName = "v1")] // 版本控制
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
    public async Task<ActionResult<Guid>> Add(AlbumAddReq req)
    {
        bool isExist = await _dbCtx.Category.AsNoTracking().AnyAsync(c => c.Id == req.CategoryId);
        if (!isExist)
            return NotFound($"The categoryId={req.CategoryId} don't exist");

        var album = await _domainService.AddAlbumAsync(req.CategoryId, req.Name);
        await _dbCtx.AddAsync(album);
        return Ok(album.Id);
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult> DeleteById(Guid id)
    {
        var album = await _repository.GetAlbumByIdAsync(id);
        if (album == null)
            return NotFound($"The album isn't found with albumId={id}");
        album.SoftDelete();
        return Ok();
    }

    [HttpPut]
    public async Task<ActionResult> Update(AlbumUpdateReq req)
    {
        var album = await _repository.GetAlbumByIdAsync(req.Id);
        if (album == null)
            return NotFound($"The album isn't found with albumId={req.Id}");
        album.ChangeName(req.Name);
        return Ok();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<Album>> FindById(Guid id)
    {
        var album = await _repository.GetAlbumByIdAsync(id);
        if (album == null)
            return NotFound($"The album isn't found with albumId={id}");
        return Ok(album);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<Album[]>> FindAllByCategoryId(Guid id)
    {
        var albums = await _repository.GetAlbumsByCategoryIdAsync(id);
        return albums;
    }

    [HttpPost]
    public async Task<ActionResult> Sort(AlbumsSortReq req)
    {
        await _domainService.SortAlbumsAsync(req.CategoryId, req.Ids);
        return Ok();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult> Show(Guid id)
    {
        var album = await _repository.GetAlbumByIdAsync(id);
        if (album == null)
            return NotFound($"The album isn't found with albumId={id}");
        album.Show();
        return Ok();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult> Hide(Guid id)
    {
        var album = await _repository.GetAlbumByIdAsync(id);
        if (album == null)
            return NotFound($"The album isn't found with albumId={id}");
        album.Hide();
        return Ok();
    }
}