using PureGym.Domain.Interfaces;

namespace PureGym.Domain.Extensions;

public static class SoftDeleteExtensions
{
    public static IQueryable<T> ExcludeDeleted<T>(this IQueryable<T> query) where T : ISoftDeletable
        => query.Where(e => !e.IsDeleted);

    public static IQueryable<T> OnlyDeleted<T>(this IQueryable<T> query) where T : ISoftDeletable
        => query.Where(e => e.IsDeleted);

    public static IEnumerable<T> ExcludeDeleted<T>(this IEnumerable<T> source) where T : ISoftDeletable
        => source.Where(e => !e.IsDeleted);

    public static IEnumerable<T> OnlyDeleted<T>(this IEnumerable<T> source) where T : ISoftDeletable
        => source.Where(e => e.IsDeleted);
}
