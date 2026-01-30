using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PureGym.Application.Interfaces;
using PureGym.Application.Models;
using PureGym.Domain.Enums;

namespace PureGym.Application.Features.Membership;

public static class AddMembership
{
    public sealed record Request(Guid MemberId, Guid MembershipTypeId);

    public sealed record Command(Guid MemberId, Guid MembershipTypeId) : IRequest<Result<Response>>;

    public sealed record Response(
        Guid MembershipId,
        Guid MemberId,
        Guid MembershipTypeId,
        string MembershipTypeName,
        DateTime StartDateUtc,
        DateTime EndDateUtc,
        MembershipStatus Status);

    public sealed class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.MemberId)
                .NotEmpty()
                .WithMessage("Member ID is required.");

            RuleFor(x => x.MembershipTypeId)
                .NotEmpty()
                .WithMessage("Membership Type ID is required.");
        }
    }

    public sealed class Handler : IRequestHandler<Command, Result<Response>>
    {
        private readonly IApplicationDbContext _dbContext;

        public Handler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<Response>> Handle(Command request, CancellationToken ct)
        {
            var member = await _dbContext.Members
                .FirstOrDefaultAsync(m => m.Id == request.MemberId && !m.IsDeleted, ct);

            if (member is null)
            {
                return Result<Response>.Failure(new Error("MEM001", "Member not found."));
            }

            var membershipType = await _dbContext.MembershipTypes
                .FirstOrDefaultAsync(mt => mt.Id == request.MembershipTypeId && !mt.IsDeleted, ct);

            if (membershipType is null)
            {
                return Result<Response>.Failure(new Error("MEM002", "Membership type not found."));
            }

            if (!membershipType.IsActive)
            {
                return Result<Response>.Failure(new Error("MEM003", "Membership type is not active."));
            }

            var membership = Domain.Entities.Membership.Create(member, membershipType);

            _dbContext.Memberships.Add(membership);
            await _dbContext.SaveChangesAsync(ct);

            var response = new Response(
                membership.Id,
                membership.MemberId,
                membership.MembershipTypeId,
                membershipType.Name,
                membership.StartDateUtc,
                membership.EndDateUtc,
                membership.Status);

            return Result<Response>.Success(response);
        }
    }
}
