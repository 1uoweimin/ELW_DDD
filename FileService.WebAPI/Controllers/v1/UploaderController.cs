using FileService.Domain;
using FileService.Domain.Entities;
using FileService.Infrastructure;
using FileService.WebAPI.Controllers.v1.Requests;
using FileService.WebAPI.Controllers.v1.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zack.ASPNETCore;

namespace FileService.WebAPI.Controllers.v1;

[Controller]
[Route("api/[controller]/[action]")]
[UnitOfWork(typeof(FSDbContext))]
[Authorize(Roles = "Admin")]
[ApiExplorerSettings(GroupName = "v1")] // 版本控制
public class UploaderController : ControllerBase
{
    private readonly FSDbContext _dbContext;
    private readonly IFSRepository _fsRepository;
    private readonly FSDomainService _fsDomainService;
    public UploaderController(IFSRepository fsRepository, FSDomainService fsDomainService, FSDbContext dbContext)
    {
        _fsRepository = fsRepository;
        _fsDomainService = fsDomainService;
        _dbContext = dbContext;
    }

    /// <summary>
    /// 检查是否有指定大小和散列值（sha256）完全一致的文件接口
    /// </summary>
    /// <param name="fileSizeInBytes">文件的大小</param>
    /// <param name="fileSHA256Hash">文件的散列值（sha256）</param>
    /// <returns>检查的结果</returns>
    [HttpGet]
    public async Task<ActionResult<FileExistRsp>> FileExist(long fileSizeInBytes, string fileSHA256Hash)
    {
        var uploadedItem = await _fsRepository.FindFileAsync(fileSizeInBytes, fileSHA256Hash);
        if (uploadedItem == null)
            return new FileExistRsp(false, null);
        else
            return new FileExistRsp(true, uploadedItem.RemoveUrl);
    }

    /// <summary>
    /// 上传表单数据的接口；
    /// </summary>
    /// <param name="upload">表单数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    [HttpPost]
    [RequestSizeLimit(60_000_000)] //RequestSizeLimit 和 UploadRequestValidator 双重检查。
    public async Task<ActionResult<Uri>> Upload([FromForm] UploadReq upload, CancellationToken cancellationToken)
    {
        /* Todo：
            1.做好校验，参考OSS的接口，防止被滥用。
            2.应该由应用服务器向 FileServer 申请一个上传码（可以指定申请的个数，这个接口只能供应用服务器调用），
              页面直传只能使用上传码上传一个文件，防止接口被恶意利用。应用服务器要控制发放上传码的频率。
            3.再提供一个非页面直传的接口，供服务器用。
         */

        var formFile = upload.File;
        string fileName = formFile.FileName;

        // 上传文件
        using Stream fileStream = formFile.OpenReadStream();
        (bool existed, UploadedItem uploadedItem) = await _fsDomainService.UploadAsync(fileStream, fileName, cancellationToken);

        // 如果文件第一次上传，则存储上传文件记录
        if (!existed) await _dbContext.AddAsync(uploadedItem);

        return uploadedItem.RemoveUrl;
    }
}
