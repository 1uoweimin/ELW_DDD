using CommonInitializer.ActionDatas;
using FluentValidation;

namespace FileService.WebAPI.Controllers.v2.Requests;

/* 注意（不明造成这个异常的原因）：
    这里的IFormFile参数不能命名为FormFile，不然调用FileService.SDK.NETCore的UploadAsync方法会抛出异常：
        System.Net.Http.HttpRequestException: 上传失败，状态码：InternalServerError，
        响应报文：System.NullReferenceException: Object reference not set to an instance of an object.
    
 */
public record UploadReq(IFormFile File);
public class UploadRequestValidator : AbstractValidator<ApiRequest<UploadReq>>
{
    public UploadRequestValidator()
    {
        /* RequestSizeLimit 和 UploadRequestValidator 双重检查：
            RequestSizeLimit在中间件管道的早期阶段检查整个请求体的大小，而UploadRequestValidator在更晚的阶段验证上传的文件。
            将RequestSizeLimit设置得稍大一些可以确保在验证器运行之前不会由于请求体太大而拒绝整个请求。
         */

        //不用校验文件名的后缀，因为文件服务器会做好安全设置，所以即使用户上传exe、php等文件都是可以的
        long maxFileSize = 50 * 1024 * 1024;
        RuleFor(u => u.ReqData.File).NotNull().Must(f => f.Length > 0 && f.Length < maxFileSize);
    }
}