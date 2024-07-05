namespace FileService.WebAPI.Controllers.v2.Responses;
public record FileExistRsp(bool IsExist = false, Uri? RemoveUrl = null);