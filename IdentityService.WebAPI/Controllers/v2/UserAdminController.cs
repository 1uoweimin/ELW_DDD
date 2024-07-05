using CommonInitializer.ActionDatas;
using IdentityService.Domain;
using IdentityService.WebAPI.Controllers.v2.Requests;
using IdentityService.WebAPI.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zack.EventBus;

namespace IdentityService.WebAPI.Controllers.v2;

[Route("api/v2/[controller]/[action]")]
[ApiController]
[Authorize(Roles = "Admin")]
[ApiExplorerSettings(GroupName = "v2")] // 版本控制
public class UserAdminController : ControllerBase
{
    private readonly IIdRepository _idRepository;
    private readonly IEventBus _eventBus;
    public UserAdminController(IIdRepository idRepository, IEventBus eventBus)
    {
        _idRepository = idRepository;
        _eventBus = eventBus;
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse<UserDTO>>> FindById(Guid id)
    {
        var user = await _idRepository.FindByIdAsync(id);
        if (user == null)
            return BadRequest(ApiResponse<UserDTO>.NotFound("The user is not found"));

        return ApiResponse<UserDTO>.Succeed(UserDTO.Create(user));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<UserDTO[]>>> FindAllUser()
    {
        var users = await _idRepository.FindAllAsync();
        return ApiResponse<UserDTO[]>.Succeed(users.Select(u => UserDTO.Create(u)).ToArray());
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> AddUser(ApiRequest<AddUserReq> req)
    {
        (var identityResult, var user, var password) = await _idRepository.AddUserAsync(req.ReqData.UserName, req.ReqData.PhoneNumber, req.ReqData.RoleType);
        if (!identityResult.Succeeded)
            return BadRequest(ApiResponse<string>.Fail(identityResult.ErrorsString()));

        // 生成的密码短信发给对方
        // 可以同时或者选择性的把新增用户的密码短信/邮件/打印给用户
        // 体现了领域事件对于代码“高内聚、低耦合”的追求
        var userCreateEvent = new UserCreateEvent(user!.Id, user.UserName!, password!, req.ReqData.PhoneNumber);
        _eventBus.Publish("IdentityService.User.Created", userCreateEvent);

        return ApiResponse<string>.Succeed();
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteUser(Guid id)
    {
        var result = await _idRepository.RemoveAsync(id);
        if (!result.Succeeded)
            return BadRequest(ApiResponse<string>.Fail(result.ErrorsString()));

        return ApiResponse<string>.Succeed();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> ChangeUserPhone(Guid id, string phoneNumber)
    {
        var user = await _idRepository.FindByIdAsync(id);
        if (user == null)
            return NotFound(ApiResponse<string>.NotFound("The user is not found"));

        var token = await _idRepository.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
        var result = await _idRepository.ChangePhoneNumberAsync(id, phoneNumber, token);
        if (!result.Succeeded)
            return BadRequest(ApiResponse<string>.Fail(result.ErrorsString()));

        return ApiResponse<string>.Succeed();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> ChangeUserEmail(Guid id, string email)
    {
        var user = await _idRepository.FindByIdAsync(id);
        if (user == null)
            return NotFound(ApiResponse<string>.NotFound("The user is not found"));

        var token = await _idRepository.GenerateChangeEmailTokenAsync(user, email);
        var result = await _idRepository.ChangeEmailAsync(id, email, token);
        if (!result.Succeeded)
            return BadRequest(ApiResponse<string>.Fail(result.ErrorsString()));

        return ApiResponse<string>.Succeed();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> ResetUserPassword(Guid id)
    {
        var user = await _idRepository.FindByIdAsync(id);
        if (user == null)
            return NotFound(ApiResponse<string>.NotFound("The user is not found"));

        string password = _idRepository.GeneratePassword();
        string token = await _idRepository.GeneratePasswordResetTokenAsync(user);
        var result = await _idRepository.ResetPasswordAsync(id, password, token);
        if (!result.Succeeded)
            return BadRequest(ApiResponse<string>.Fail(result.ErrorsString()));

        //生成的密码短信发给对方
        var eventData = new ResetPasswordEvent(user.Id, user.UserName!, password, user.PhoneNumber!);
        _eventBus.Publish("IdentityService.User.PasswordReset", eventData);
        return ApiResponse<string>.Succeed();
    }
}