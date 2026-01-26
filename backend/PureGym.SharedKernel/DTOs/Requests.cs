namespace PureGym.SharedKernel.DTOs;

public sealed record VerifyAccessRequest(
    Guid MemberId,
    string AccessKey);
