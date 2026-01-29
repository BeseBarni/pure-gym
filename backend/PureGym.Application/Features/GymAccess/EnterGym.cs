using FluentValidation;
using MassTransit;
using MediatR;
using PureGym.Application.Interfaces.Services;
using PureGym.Application.Models;
using PureGym.SharedKernel.Constants;
using PureGym.SharedKernel.Events;
using PureGym.SharedKernel.Models;

namespace PureGym.Application.Features.GymAccess;

public static class EnterGym
{
    public sealed record Request(Guid MemberId, string AccessKey);

    public sealed record Command(Guid MemberId, string AccessKey) : IRequest<Result<Response>>;

    public sealed record Response(
        Guid MemberId,
        DateTime AccessedAtUtc);

    public sealed class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.MemberId)
                .NotEmpty()
                .WithMessage("Member ID is required.");
            RuleFor(x => x.AccessKey)
                .NotEmpty()
                .WithMessage("AccessKey is required.");
        }
    }

    public sealed class Handler : IRequestHandler<Command, Result<Response>>
    {
        private readonly ICacheService _cacheService;
        private readonly IPublishEndpoint _publishEndpoint;
        public Handler(ICacheService cacheService, IPublishEndpoint publishEndpoint)
        {
            _cacheService = cacheService;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Result<Response>> Handle(Command request, CancellationToken ct)
        {
            var key = CacheKeys.MemberAccess(request.MemberId);

            var cachedAccessKey = await _cacheService.GetAsync<CachedEntry<string>>(key, ct);

            if (cachedAccessKey is null || cachedAccessKey.ExpiresAt < DateTime.UtcNow || cachedAccessKey.Data != request.AccessKey)
            {
                await _cacheService.RemoveAsync(key, ct);
                return Result<Response>.Failure(new Error("AC1", "Entrance denied, access key doesnt match."));
            }

            var accessedAt = DateTime.UtcNow;

            await _publishEndpoint.Publish(
                new GymAccessGrantedEvent(
                    request.MemberId,
                    accessedAt),
                ct);

            var response = new Response(
                request.MemberId,
                accessedAt
            );

            return Result<Response>.Success(response);
        }
    }
}
