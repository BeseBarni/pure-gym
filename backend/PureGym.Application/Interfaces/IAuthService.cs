using PureGym.SharedKernel.Results;

namespace PureGym.Application.Interfaces;

public interface IAuthService
{
    Task<bool> EmailExistsAsync(string email);
    Task<(bool Success, Guid UserId, string[] Errors)> CreateUserAsync(string email, string password);
    Task<(bool Success, Guid UserId, string[] Roles)> CheckPasswordAsync(string email, string password);
    Task<ExternalLoginResult> GetExternalLoginInfoAsync();
    Task<(bool Success, Guid UserId)> CreateExternalUserAsync(string email, string provider, string providerKey);
}
