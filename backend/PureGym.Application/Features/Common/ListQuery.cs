using MediatR;
using Microsoft.EntityFrameworkCore;
using PureGym.Application.Interfaces;
using PureGym.Application.Models;
using PureGym.Domain.Interfaces;

namespace PureGym.Application.Features.Common;

public static class ListQuery<TEntity> where TEntity : class
{
    public sealed record Query : IRequest<Result<Response>>;

    public sealed record Response(IReadOnlyList<TEntity> Items, int TotalCount);

    public sealed class Handler : IRequestHandler<Query, Result<Response>>
    {
        private readonly IApplicationDbContext _dbContext;

        public Handler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<Response>> Handle(Query request, CancellationToken ct)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(TEntity)))
            {
                query = query.Where(e => !((ISoftDeletable)e).IsDeleted);
            }

            var items = await query.ToListAsync(ct);

            return Result<Response>.Success(new Response(items, items.Count));
        }
    }
}
