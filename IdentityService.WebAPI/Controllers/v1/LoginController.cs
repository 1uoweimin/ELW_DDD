using IdentityService.Domain;
using IdentityService.Domain.Entities;
using IdentityService.WebAPI.Controllers.v1.Requests;
using IdentityService.WebAPI.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zack.EventBus;

namespace IdentityService.WebAPI.Controllers.v1;

// 注意：在生产项目中，一定要把图形验证码或者行为验证码等形式来防止暴力破解，以保证接口的安全。

[Route("api/[controller]/[action]")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")] // 版本控制
public class LoginController : ControllerBase
{
    private readonly IdDomainService _idDomainService;
    private readonly IIdRepository _idRepository;
    private readonly IEventBus _eventBus;
    public LoginController(IdDomainService idDomainService, IIdRepository idRepository, IEventBus eventBus)
    {
        _idDomainService = idDomainService;
        _idRepository = idRepository;
        _eventBus = eventBus;
    }

    /// <summary>
    /// 测试的时候用来创建一个用户
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> InitialCreateAdminUser()
    {
        string userName = "Administrator";
        string phoneNumber = "15622059431";
        string email = "2680067640@qq.com";
        string password = "123456";

        (IdentityResult idResult, User? user, string? pwd) = await _idRepository.AddUserAsync(userName, phoneNumber, RoleType.Admin);
        if (!idResult.Succeeded)
        {
            return BadRequest(idResult.ErrorsString());
        }
        else
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            string token1 = await _idRepository.GeneratePasswordResetTokenAsync(user);
            var result1 = await _idRepository.ResetPasswordAsync(user.Id, password, token1);
            if (!result1.Succeeded)
                return BadRequest(result1.ErrorsString());

            string token2 = await _idRepository.GenerateChangeEmailTokenAsync(user, email);
            var result2 = await _idRepository.ChangeEmailAsync(user.Id, email, token2);
            if (!result2.Succeeded)
                return BadRequest(result2.ErrorsString());

            return Ok($"初始化创建管理用户\n用户名：{userName}\n密码：{password}");
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<UserDTO>> GetUserInfos()
    {
        Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var user = await _idRepository.FindByIdAsync(userId);
        if (user == null)  // 可能用户注销了
            return NotFound();
        //出于安全考虑，不要机密信息传递到客户端（除非确认没问题，否则尽量不要直接把实体类对象返回给前端）
        return Ok(UserDTO.Create(user));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RestPassword(RestPasswordReq req)
    {
        Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var user = await _idRepository.FindByIdAsync(userId);
        if (user == null)
            return NotFound("The user is not found");

        string token = await _idRepository.GeneratePasswordResetTokenAsync(user);
        var result = await _idRepository.ResetPasswordAsync(userId, req.Password, token);
        if (!result.Succeeded)
            return BadRequest(result.ErrorsString());
        else
            return Ok();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> ChangePhoneNumber(string phoneNumber)
    {
        Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var user = await _idRepository.FindByIdAsync(userId);
        if (user == null)
            return NotFound("The user is not found");

        var token = await _idRepository.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
        var result = await _idRepository.ChangePhoneNumberAsync(userId, phoneNumber, token);
        if (!result.Succeeded)
            return BadRequest(result.ErrorsString());

        return Ok();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> ChangeEmailNumber(string email)
    {
        Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var user = await _idRepository.FindByIdAsync(userId);
        if (user == null)
            return NotFound("The user is not found");

        var token = await _idRepository.GenerateChangeEmailTokenAsync(user, email);
        var result = await _idRepository.ChangeEmailAsync(userId, email, token);
        if (!result.Succeeded)
            return BadRequest(result.ErrorsString());

        return Ok();
    }

    [HttpPost]
    public async Task<ActionResult<string>> LoginByNameAndPwd(LoginByNameAndPwdReq req)
    {
        (var signInResult, var token) = await _idDomainService.LoginByNameAndPwd(req.UserName, req.Password);

        if (signInResult.IsLockedOut)
            return BadRequest($"The user has been locked");

        if (!signInResult.Succeeded)
            return BadRequest("The username or password is incorrent");

        return token!;
    }

    [HttpPost]
    public async Task<ActionResult<string>> LoginByEmailAndPwd(LoginByEmailAndPwdReq req)
    {
        (var signInResult, var token) = await _idDomainService.LoginByEmailAndPwd(req.Email, req.Password);

        if (signInResult.IsLockedOut)
            return BadRequest($"The user has been locked");

        if (!signInResult.Succeeded)
            return BadRequest("The email or password is incorrent");

        return token!;
    }

    [HttpPost]
    public async Task<ActionResult<string>> LoginByPhoneAndVerifiedCode(LoginByPhoneAndCodeReq req)
    {
        (var signInResult, var token) = await _idDomainService.LoginByPhoneAndCodeAsync(req.PhoneNumber, req.VerifiedCode);

        if (signInResult.IsLockedOut)
            return BadRequest($"The user been locked");

        if (!signInResult.Succeeded)
            return BadRequest("Verification is mismatch");

        return token!;
    }

    [HttpPost]
    public async Task<ActionResult<string>> LoginByEmailAndVerifiedCode(LoginByEmailAndCodeReq req)
    {
        (var signInResult, var token) = await _idDomainService.LoginByEmailAndCodeAsync(req.Email, req.VerifiedCode);

        if (signInResult.IsLockedOut)
            return BadRequest($"The user been locked");

        if (!signInResult.Succeeded)
            return BadRequest("Verification is mismatch");

        return token!;
    }

    [HttpGet]
    public async Task<ActionResult<string>> CreatePhoneVerifiedCode(string phoneNumber)
    {
        (var result, string? code) = await _idDomainService.CreatePhoneCodeAsync(phoneNumber);
        if (!result.Succeeded)
            return BadRequest(result.ErrorsString());

        var eventData = new SendPhoneVerifiedCodeEvent(phoneNumber, code!);
        _eventBus.Publish("IdentityService.Login.PhoneVerifiedCode", eventData);

        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<string>> CreateEmailVerifiedCode(string email)
    {
        (var result, string? code) = await _idDomainService.CreateEmailCodeAsync(email);
        if (!result.Succeeded)
            return BadRequest(result.ErrorsString());

        var eventData = new SendEmailVerifiedCodeEvent(email, code!);
        _eventBus.Publish("IdentityService.Login.EmailVerifiedCode", eventData);

        return Ok();
    }
}