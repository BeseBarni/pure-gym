using MediatR;
using Microsoft.EntityFrameworkCore;
using PureGym.Application.Interfaces;
using PureGym.Application.Interfaces.Requests;

namespace PureGym.Application.Validators;

public sealed class MembershipValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IMemberRequest
{
    private readonly IApplicationDbContext _context;
    public MembershipValidationBehavior(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var member = await _context
            .Members
            .Include(p => p.Memberships)
            .FirstOrDefaultAsync(p => p.Id == request.MemberId);

        if (member is null) throw new UnauthorizedAccessException("Active membership required.");

        if (member.GetActiveMembership() is null)
        {
            throw new UnauthorizedAccessException("Active membership required.");
        }

        return await next();
    }
}
