using System.Net.Http.Headers;
using System.Security.Claims;
using Zack.Commons;
using Zack.JWT;

namespace FileService.SDK.NETCore;

public class FileServiceClient
{
    private IHttpClientFactory _httpClientFactory;
    private Uri _webRootUrl;
    private ITokenService _tokenService;
    private JWTOptions _jwTOptions;
    public FileServiceClient(IHttpClientFactory httpClientFactory, Uri webRootUrl, ITokenService tokenService, JWTOptions jwTOptions)
    {
        _httpClientFactory = httpClientFactory;
        _webRootUrl = webRootUrl;
        _tokenService = tokenService;
        _jwTOptions = jwTOptions;
    }

    /// <summary>
    /// 生成JWT值；
    /// 因为JWT的key等机密信息只有服务器端知道，因此可以这样非常简单的读到配置。
    /// </summary>
    /// <returns></returns>
    private string BuildToken()
    {
        IEnumerable<Claim> claims = new List<Claim>() { new Claim(ClaimTypes.Role, "Admin") };
        return _tokenService.BuildToken(claims, _jwTOptions);
    }

    /// <summary>
    /// 检查是否有指定大小和散列值（sha256）完全一致的文件接口
    /// </summary>
    /// <param name="fileSizeInBytes">文件的大小</param>
    /// <param name="fileSHA256Hash">文件的散列值（sha256）</param>
    /// <returns>检查的结果</returns>
    public async Task<FileExistResponse?> FileExistAsync(long fileSizeInBytes, string fileSHA256Hash)
    {
        // 请求路径
        string relativeUrl = FormattableStringHelper.BuildUrl($"api/Uploader/FileExist?fileSizeInBytes={fileSizeInBytes}&fileSHA256Hash={fileSHA256Hash}");
        Uri fileExistUrl = new Uri(_webRootUrl, relativeUrl);

        // 执行请求并返回结果
        using var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", BuildToken());
        return await httpClient.GetJsonAsync<FileExistResponse>(fileExistUrl);
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="fileInfo">文件信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    public async Task<Uri> UploadAsync(FileInfo fileInfo, CancellationToken cancellationToken)
    {
        // 请求路径
        string relativeUrl = FormattableStringHelper.BuildUrl($"api/Uploader/Upload");
        Uri uploadUrl = new Uri(_webRootUrl, relativeUrl);

        // 请求体
        using MultipartFormDataContent content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(fileInfo.OpenRead());
        content.Add(fileContent, "file", fileInfo.Name);

        // 执行请求
        using var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", BuildToken());
        using var responseMessage = await httpClient.PostAsync(uploadUrl, content, cancellationToken);

        // 返回结果
        string respStr = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
        if (responseMessage.IsSuccessStatusCode)
            return respStr.ParseJson<Uri>()!;
        else
            throw new HttpRequestException($"上传失败，状态码：{responseMessage.StatusCode}\n响应报文：{respStr}");
    }
}

public record FileExistResponse(bool isExist, Uri? removeUrl);
