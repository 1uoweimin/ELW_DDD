using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Domain;

/// <summary>
/// 仓储接口
/// </summary>
public interface IIdRepository
{
    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <param name="phoneNumber">手机号</param>
    /// <param name="isAdministrator">isAdministrator = false表示为普通用户；否则为管理员</param>
    /// <returns></returns>
    Task<(IdentityResult, User?, string? password)> AddUserAsync(string userName, string phoneNumber, RoleType roleType);
    /// <summary>
    /// 软删除用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns></returns>
    Task<IdentityResult> RemoveAsync(Guid id);
    /// <summary>
    /// 通过用户ID查找用户
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns></returns>
    Task<User?> FindByIdAsync(Guid userId);
    /// <summary>
    /// 通过用户名查找用户
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <returns></returns>
    Task<User?> FindByNameAsync(string userName);
    /// <summary>
    /// 通过用户邮箱查找用户
    /// </summary>
    /// <param name="email">用户邮箱</param>
    /// <returns></returns>
    Task<User?> FindByEmailAsync(string email);
    /// <summary>
    /// 通过用户手机号查找用户
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    Task<User?> FindByPhoneAsync(string phoneNumber);
    /// <summary>
    /// 查找所有用户
    /// </summary>
    /// <returns></returns>
    Task<User[]> FindAllAsync();

    /// <summary>
    /// 把user加入角色role
    /// </summary>
    /// <param name="user"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    Task<IdentityResult> AddToRoleAsync(User user, RoleType roleType);
    /// <summary>
    /// 获得用户的所有角色
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<IList<string>> GetRolesAsync(User user);

    /// <summary>
    /// 为了登录检查用户、密码是否正确
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <param name="isAccessFail">isAccessFail = true 表示如果登录失败，则记录一次登陆失败</param>
    /// <returns></returns>
    Task<SignInResult> CheckForSignInAsync(User user, string password, bool isAccessFail = true);

    /// <summary>
    /// 重新设置密码
    /// </summary>
    /// <param name="id"></param>
    /// <param name="newPassword"></param>
    /// <returns></returns>
    Task<IdentityResult> ResetPasswordAsync(Guid id, string newPassword, string token);
    /// <summary>
    /// 改变用户手机的号码
    /// </summary>
    /// <param name="id"></param>
    /// <param name="newPhone"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<IdentityResult> ChangePhoneNumberAsync(Guid id, string newPhone, string token);
    /// <summary>
    /// 重新设置邮箱
    /// </summary>
    /// <param name="id"></param>
    /// <param name="newEmail"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<IdentityResult> ChangeEmailAsync(Guid id, string newEmail, string token);

    /// <summary>
    /// 生成重设密码token
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<string> GeneratePasswordResetTokenAsync(User user);
    /// <summary>
    /// 生成重置手机号的token
    /// </summary>
    /// <param name="user"></param>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    Task<string> GenerateChangePhoneNumberTokenAsync(User user, string newPhoneNumber);
    /// <summary>
    /// 生成重置邮箱的token
    /// </summary>
    /// <param name="user"></param>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    Task<string> GenerateChangeEmailTokenAsync(User user, string newEmail);

    /// <summary>
    /// 生成密码
    /// </summary>
    /// <returns></returns>
    string GeneratePassword();
    /// <summary>
    /// 用户是否被锁定
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<bool> IsLockedOutAsync(User user);
}

/// <summary>
/// 角色的类型
/// </summary>
public enum RoleType { Admin = 0, User = 1 }