namespace StayNGo.Api.Features.Common;

public abstract class PaginationFilter
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }

    public int EffectivePage => Math.Max(1, Page ?? 1);
    public int EffectivePageSize => Math.Clamp(PageSize ?? 10, 1, 100);
}