using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Zack.JWT;

namespace IdentityService.Domain;

/// <summary>
/// 认证领域服务：领域服务只有抽象的业务逻辑。
/// </summary>
public class IdDomainService
{
    private readonly IIdRepository _idRepository;
    private readonly IDistributedCache _distributedCache;
    private readonly ITokenService _tokenService;
    private readonly IOptionsSnapshot<JWTOptions> _jwtOptions;
    public IdDomainService(IIdRepository idRepository, IDistributedCache distributedCache, ITokenService tokenService, IOptionsSnapshot<JWTOptions> jwtOptions)
    {
        _idRepository = idRepository;
        _distributedCache = distributedCache;
        _tokenService = tokenService;
        _jwtOptions = jwtOptions;
    }

    // 元组语法：例如 Task<(SignInResult, string?)> 可以返回多个参数

    private async Task<string> GenerateTokenAsync(User user)
    {
        List<Claim> claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        foreach (var role in await _idRepository.GetRolesAsync(user))
            claims.Add(new Claim(ClaimTypes.Role, role));

        return _tokenService.BuildToken(claims, _jwtOptions.Value);
    }
    private string GetKeyForVerifiedCode(string phoneNumberOrEmail) => $"IdentityService_{phoneNumberOrEmail}_VerifiedCode";
    private async Task<string> GenerateVerifiedCodeAsync(string phoneNumberOrEmail)
    {
        string code = string.Empty;
        for (int i = 0; i < 6; i++)
            code += Random.Shared.Next(0, 10).ToString();
        string key = GetKeyForVerifiedCode(phoneNumberOrEmail);
        DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(60)
        };
        await _distributedCache.SetStringAsync(key, code, options);
        return code;
    }
    private async Task<(SignInResult, string? token)> LoginByUserAndPwdAsync(User user, string password)
    {
        var result = await _idRepository.CheckForSignInAsync(user, password);
        if (result.Succeeded)
            return (SignInResult.Success, await GenerateTokenAsync(user));
        else
            return (result, null);
    }
    private async Task<(SignInResult, string? token)> LoginByVerifiedCodeAsync(User user, string phoneNumberOrEmail, string verifiedCode)
    {
        if (await _idRepository.IsLockedOutAsync(user))
            return (SignInResult.LockedOut, null);

        string key = GetKeyForVerifiedCode(phoneNumberOrEmail);
        var verifiedCodeInCache = await _distributedCache.GetStringAsync(key);
        if (verifiedCodeInCache == null || verifiedCodeInCache != verifiedCode)
            return (SignInResult.Failed, null);

        return (SignInResult.Success, await GenerateTokenAsync(user));
    }

    public async Task<(IdentityResult, string? verifiedCode)> CreatePhoneCodeAsync(string phoneNumber)
    {
        var user = await _idRepository.FindByPhoneAsync(phoneNumber);
        if (user == null)
            return ($"The user is not exist with phoneNumber equal {phoneNumber}".ErrorIdentityResult(), null);

        string code = await GenerateVerifiedCodeAsync(phoneNumber);
        return (IdentityResult.Success, code);
    }
    public async Task<(IdentityResult, string? verifiedCode)> CreateEmailCodeAsync(string email)
    {
        var user = await _idRepository.FindByEmailAsync(email);
        if (user == null)
            return ($"The user is not exist with email equal {email}".ErrorIdentityResult(), null);

        string code = await GenerateVerifiedCodeAsync(email);
        return (IdentityResult.Success, code);
    }

    public async Task<(SignInResult, string? token)> LoginByNameAndPwd(string userName, string password)
    {
        var user = await _idRepository.FindByNameAsync(userName);
        if (user == null)
            return (SignInResult.Failed, null);

        return await LoginByUserAndPwdAsync(user, password);

    }
    public async Task<(SignInResult, string? token)> LoginByEmailAndPwd(string email, string password)
    {
        var user = await _idRepository.FindByEmailAsync(email);
        if (user == null)
            return (SignInResult.Failed, null);

        return await LoginByUserAndPwdAsync(user, password);
    }
    public async Task<(SignInResult, string? token)> LoginByPhoneAndCodeAsync(string phoneNumber, string verifiedCode)
    {
        var user = await _idRepository.FindByPhoneAsync(phoneNumber);
        if (user == null)
            return (SignInResult.Failed, null);

        return await LoginByVerifiedCodeAsync(user, phoneNumber, verifiedCode);
    }
    public async Task<(SignInResult, string? token)> LoginByEmailAndCodeAsync(string email, string verifiedCode)
    {
        var user = await _idRepository.FindByEmailAsync(email);
        if (user == null)
            return (SignInResult.Failed, null);

        return await LoginByVerifiedCodeAsync(user, email, verifiedCode);
    }
}