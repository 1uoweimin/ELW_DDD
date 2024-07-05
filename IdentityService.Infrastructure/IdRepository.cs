using IdentityService.Domain;
using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace IdentityService.Infrastructure;

/// <summary>
/// 仓储服务实现
/// </summary>
public class IdRepository : IIdRepository
{
    private readonly IdUserManager _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IdDbContext _dbContext;
    public IdRepository(IdUserManager userManager, RoleManager<Role> roleManager, IdDbContext dbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
    }

    public async Task<(IdentityResult, User?, string? password)> AddUserAsync(string userName, string phoneNumber, RoleType roleType)
    {
        // 用户名是不可以看重复的（包括已经软删除的用户）
        var user = _dbContext.Set<User>().AsNoTracking().IgnoreQueryFilters().FirstOrDefault(u => u.UserName == userName);
        if (user != null)
            return ("The userName is alreadly exist".ErrorIdentityResult(), null, null);
        if (await FindByPhoneAsync(phoneNumber) != null)
            return ("The phoneNumber is alreadly exist".ErrorIdentityResult(), null, null);

        user = new User(userName);
        user.PhoneNumber = phoneNumber;
        user.PhoneNumberConfirmed = true;
        string password = GeneratePassword();

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return (result, null, null);

        result = await AddToRoleAsync(user, roleType);
        if (!result.Succeeded)
            return (result, null, null);

        return (IdentityResult.Success, user, password);
    }
    public async Task<IdentityResult> RemoveAsync(Guid id)
    {
        var user = await FindByIdAsync(id);
        if (user == null)
            throw new NullReferenceException($"The user can not Null with Id = {id}");

        //一定要删除aspnetuserlogins表中的数据，否则再次用这个外部登录登录的话
        //就会报错：The instance of entity type 'IdentityUserLogin<Guid>' cannot be tracked because another instance with the same key value for {'LoginProvider', 'ProviderKey'} is already being tracked.
        //而且要先删除aspnetuserlogins数据，再软删除User
        var noneCT = default(CancellationToken);
        var userLoginStore = _userManager.UserLoginStore;
        foreach (var login in await userLoginStore.GetLoginsAsync(user, noneCT))
            await userLoginStore.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey, noneCT);

        user.SoftDelete();
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return IdentityResult.Failed(result.Errors.ToArray());

        return IdentityResult.Success;
    }
    public Task<User?> FindByIdAsync(Guid userId) => _userManager.FindByIdAsync(userId.ToString());
    public Task<User?> FindByNameAsync(string userName) => _userManager.FindByNameAsync(userName);
    public Task<User?> FindByEmailAsync(string email) => _userManager.FindByEmailAsync(email);
    public Task<User?> FindByPhoneAsync(string phoneNumber) => _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
    public Task<User[]> FindAllAsync() => _userManager.Users.ToArrayAsync();

    public async Task<IdentityResult> AddToRoleAsync(User user, RoleType roleType)
    {
        string roleName = roleType.ToString();
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            var resutl = await _roleManager.CreateAsync(new Role() { Name = roleName });
            if (!resutl.Succeeded)
                return resutl;
        }

        return await _userManager.AddToRoleAsync(user, roleName);
    }
    public Task<IList<string>> GetRolesAsync(User user) => _userManager.GetRolesAsync(user);

    public async Task<SignInResult> CheckForSignInAsync(User user, string password, bool isAccessFail)
    {
        var isLockOut = await IsLockedOutAsync(user);
        if (isLockOut)
            return SignInResult.LockedOut;

        var matchPwd = await _userManager.CheckPasswordAsync(user, password);
        if (matchPwd)
        {
            var r = await _userManager.ResetAccessFailedCountAsync(user);
            if (!r.Succeeded)
                throw new ApplicationException("ResetAccessFailCount failed");
            return SignInResult.Success;
        }
        else
        {
            if (isAccessFail)
            {
                var r = await _userManager.AccessFailedAsync(user);
                if (!r.Succeeded)
                    throw new ApplicationException("AccessFailed failed");
            }

            return SignInResult.Failed;
        }
    }

    public async Task<IdentityResult> ResetPasswordAsync(Guid id, string newPassword, string token)
    {
        var user = await FindByIdAsync(id);
        if (user == null)
            return "The user is not exist".ErrorIdentityResult("404");

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        return result;
    }
    public async Task<IdentityResult> ChangePhoneNumberAsync(Guid id, string newPhone, string token)
    {
        var user = await FindByIdAsync(id);
        if (user == null)
            return "The user is not exist".ErrorIdentityResult("404");

        if (await FindByPhoneAsync(newPhone) != null)
            return $"The phoneNubmer={newPhone} already exists".ErrorIdentityResult();

        var result = await _userManager.ChangePhoneNumberAsync(user, newPhone, token);

        return result;
    }
    public async Task<IdentityResult> ChangeEmailAsync(Guid id, string newEmail, string token)
    {
        var user = await FindByIdAsync(id);
        if (user == null)
            return "The user is not exist".ErrorIdentityResult("404");

        if (await FindByEmailAsync(newEmail) != null)
            return $"The email={newEmail} already exists".ErrorIdentityResult();

        var result = await _userManager.ChangeEmailAsync(user, newEmail, token);

        return result;
    }

    public Task<string> GeneratePasswordResetTokenAsync(User user) => _userManager.GeneratePasswordResetTokenAsync(user);
    public Task<string> GenerateChangePhoneNumberTokenAsync(User user, string newPhoneNumber) => _userManager.GenerateChangePhoneNumberTokenAsync(user, newPhoneNumber);
    public Task<string> GenerateChangeEmailTokenAsync(User user, string newEmail) => _userManager.GenerateChangeEmailTokenAsync(user, newEmail);

    public string GeneratePassword()
    {
        var options = _userManager.Options.Password;
        int length = options.RequiredLength;
        bool nonAlphanumeric = options.RequireNonAlphanumeric;
        bool digit = options.RequireDigit;
        bool lowercase = options.RequireLowercase;
        bool uppercase = options.RequireUppercase;

        // ASCII 码
        StringBuilder passwordSB = new StringBuilder();
        Random random = new Random();
        while (passwordSB.Length < random.Next(length, length * 2))
        {
            char c = (char)random.Next(32, 126);
            passwordSB.Append(c);
            if (char.IsDigit(c)) digit = false;
            else if (char.IsLower(c)) lowercase = false;
            else if (char.IsUpper(c)) uppercase = false;
            else if (!char.IsLetterOrDigit(c)) nonAlphanumeric = false;
        }

        if (nonAlphanumeric) //非字母数字字符
            passwordSB.Append((char)random.Next(33, 48));
        if (digit) // 数字0-9（48-57）
            passwordSB.Append((char)random.Next(48, 58));
        if (lowercase) // 小写字母a-z（97-122）
            passwordSB.Append((char)random.Next(97, 123));
        if (uppercase) // 大写字母A-Z（65-90）
            passwordSB.Append((char)random.Next(65, 91));
        return passwordSB.ToString();
    }
    public Task<bool> IsLockedOutAsync(User user) => _userManager.IsLockedOutAsync(user);
}
