using IdentityService.Domain;
using IdentityService.WebAPI.Controllers.v1.Requests;
using IdentityService.WebAPI.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zack.EventBus;

namespace IdentityService.WebAPI.Controllers.v1;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize(Roles = "Admin")]
[ApiExplorerSettings(GroupName = "v1")] // 版本控制
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
    public async Task<ActionResult<UserDTO>> FindById(Guid id)
    {
        var user = await _idRepository.FindByIdAsync(id);
        if (user == null) return NotFound();
        return UserDTO.Create(user);
    }

    [HttpGet]
    public async Task<ActionResult<UserDTO[]>> FindAllUser()
    {
        var users = await _idRepository.FindAllAsync();
        return users.Select(u => UserDTO.Create(u)).ToArray();
    }

    [HttpPost]
    public async Task<IActionResult> AddUser(AddUserReq req)
    {
        (var identityResult, var user, var password) = await _idRepository.AddUserAsync(req.UserName, req.PhoneNumber, req.RoleType);
        if (!identityResult.Succeeded)
            return BadRequest(identityResult.ErrorsString());

        // 生成的密码短信发给对方
        // 可以同时或者选择性的把新增用户的密码短信/邮件/打印给用户
        // 体现了领域事件对于代码“高内聚、低耦合”的追求
        var userCreateEvent = new UserCreateEvent(user!.Id, user.UserName!, password!, req.PhoneNumber);
        _eventBus.Publish("IdentityService.User.Created", userCreateEvent);
        return Ok();
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var result = await _idRepository.RemoveAsync(id);
        if (!result.Succeeded)
            return NotFound(result.ErrorsString());
        return Ok();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> ChangeUserPhone(Guid id, string phoneNumber)
    {
        var user = await _idRepository.FindByIdAsync(id);
        if (user == null)
            return NotFound("The user is not found");

        var token = await _idRepository.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
        var result = await _idRepository.ChangePhoneNumberAsync(id, phoneNumber, token);
        if (!result.Succeeded)
            return BadRequest(result.ErrorsString());

        return Ok();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> ChangeUserEmail(Guid id, string email)
    {
        var user = await _idRepository.FindByIdAsync(id);
        if (user == null)
            return NotFound("The user is not found");

        var token = await _idRepository.GenerateChangeEmailTokenAsync(user, email);
        var result = await _idRepository.ChangeEmailAsync(id, email, token);
        if (!result.Succeeded)
            return BadRequest(result.ErrorsString());

        return Ok();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> ResetUserPassword(Guid id)
    {
        var user = await _idRepository.FindByIdAsync(id);
        if (user == null)
            return NotFound("The user is not found");

        string password = _idRepository.GeneratePassword();
        string token = await _idRepository.GeneratePasswordResetTokenAsync(user);
        var result = await _idRepository.ResetPasswordAsync(id, password, token);
        if (!result.Succeeded)
            return BadRequest(result.ErrorsString());

        //生成的密码短信发给对方
        var eventData = new ResetPasswordEvent(user.Id, user.UserName!, password, user.PhoneNumber!);
        _eventBus.Publish("IdentityService.User.PasswordReset", eventData);
        return Ok();
    }
}