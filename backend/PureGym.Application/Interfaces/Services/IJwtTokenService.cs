namespace PureGym.Application.Interfaces.Services;

public interface IJwtTokenService
{
    string GenerateToken(Guid userId, string email, IList<string> roles);
}
