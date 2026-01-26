using Microsoft.AspNetCore.Identity;
using PureGym.Application.Interfaces;
using PureGym.Infrastructure.Persistence;
using PureGym.SharedKernel.Results;
using System.Security.Claims;

namespace PureGym.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<(bool Success, Guid UserId, string[] Roles)> CheckPasswordAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return (false, Guid.Empty, Array.Empty<string>());

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

        if (!result.Succeeded) return (false, Guid.Empty, Array.Empty<string>());

        var roles = await _userManager.GetRolesAsync(user);
        return (true, user.Id, roles.ToArray());
    }

    public async Task<(bool Success, Guid UserId)> CreateExternalUserAsync(string email, string provider, string providerKey)
    {
        var user = new ApplicationUser { UserName = email, Email = email };
        var result = await _userManager.CreateAsync(user);

        if (!result.Succeeded) return (false, Guid.Empty);

        await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerKey, provider));

        return (true, user.Id);
    }

    public async Task<(bool Success, Guid UserId, string[] Errors)> CreateUserAsync(string email, string password)
    {
        var user = new ApplicationUser { UserName = email, Email = email };
        var result = await _userManager.CreateAsync(user, password);

        return (result.Succeeded, user.Id, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<bool> EmailExistsAsync(string email) => await _userManager.FindByEmailAsync(email) != null;

    public async Task<ExternalLoginResult> GetExternalLoginInfoAsync()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null) return new ExternalLoginResult("", "", "", false);

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        return new ExternalLoginResult(email!, info.LoginProvider, info.ProviderKey, true);
    }
}
