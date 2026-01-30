using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;
using PureGym.Application.Interfaces.Requests;
using PureGym.Application.Interfaces.Services;
using PureGym.Application.Settings;
using PureGym.SharedKernel.Constants;

namespace PureGym.Application.Features.GymAccess;

public static class RequestEntryQR
{
    public sealed record Request(Guid MemberId);
    public sealed record Command(Guid MemberId) : IRequest<Response>, IMemberRequest;
    public sealed record Response(Guid MemberId, string? EntryCode, DateTime? Expiry);

    public sealed class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.MemberId)
                .NotEmpty()
                .WithMessage("Member ID is required.");
        }
    }

    public sealed class Handler : IRequestHandler<Command, Response>
    {
        private readonly ICacheService _cacheService;
        private readonly GymEntrySettings _settings;

        public Handler(ICacheService cacheService, IOptions<GymEntrySettings> options)
        {
            _cacheService = cacheService;
            _settings = options.Value;
        }

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var key = CacheKeys.MemberAccess(request.MemberId);
            var ttl = TimeSpan.FromSeconds(_settings.EntryKeyCacheTime);

            var result = await _cacheService.GetOrSetAsync(
                key,
                factory: () =>
                {
                    var code = Guid.NewGuid().ToString("N")[..6].ToUpper();
                    return Task.FromResult(code);
                },
                expiration: ttl,
                cancellationToken: cancellationToken
            );
            return new Response(request.MemberId, result.Data, result.ExpiresAt);
        }
    }
}
