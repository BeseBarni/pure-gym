using FluentValidation;
using MediatR;
using PureGym.Application.Interfaces;

namespace PureGym.Application.Features.GymAccess;

public static class RequestEntryCode
{
    public sealed record Request(Guid MemberId);
    public sealed record Command(Guid MemberId) : IRequest<Response>;
    public sealed record Response(Guid MemberId, string EntryCode);

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
        private readonly IApplicationDbContext _context;

        public Handler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {

            return new Response(request.MemberId, "1234");
        }
    }
}
