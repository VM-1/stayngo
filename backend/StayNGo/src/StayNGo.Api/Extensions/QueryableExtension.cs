using StayNGo.Api.Features.Common;

namespace StayNGo.Api.Extensions;

public static class QueryableExtension
{
    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, PaginationFilter filter)
    {
        var page = Math.Max(1, filter.Page);
        var size = Math.Clamp(filter.PageSize, 1, 100);
        return query.Skip((page - 1) * size).Take(size);
    }
}