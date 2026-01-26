namespace PureGym.SharedKernel.Results;

public record ExternalLoginResult(string Email, string Provider, string ProviderKey, bool IsAuthenticated);
