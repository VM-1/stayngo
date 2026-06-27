namespace StayNGo.Api.Features.Common;

public record PageResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int Total)
{
    public PageResult(IReadOnlyList<T> items, PaginationFilter filter, int total)
        : this(items, filter.EffectivePage, filter.EffectivePageSize, total)
    {
    }
}