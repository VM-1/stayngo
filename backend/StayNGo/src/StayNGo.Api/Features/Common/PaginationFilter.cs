namespace StayNGo.Api.Features.Common;

public abstract class PaginationFilter
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}