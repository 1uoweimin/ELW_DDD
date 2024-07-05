namespace FileService.WebAPI.Controllers.v1.Responses;
public record FileExistRsp(bool IsExist, Uri? RemoveUrl);