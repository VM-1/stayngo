using StayNGo.Api.Features.Common;

namespace StayNGo.Api.Extensions;

public static class QueryableExtension
{
    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, PaginationFilter filter)
    {
        return query.Skip((filter.EffectivePage - 1) * filter.EffectivePageSize).Take(filter.EffectivePageSize);
    }
}