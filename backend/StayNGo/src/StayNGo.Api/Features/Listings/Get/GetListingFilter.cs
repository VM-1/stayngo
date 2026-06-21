using StayNGo.Api.Features.Common;

namespace StayNGo.Api.Features.Listings.Get;

public class GetListingFilter : PaginationFilter
{
    public string? Location { get; set; }
    public int? MinCapacity { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Currency { get; set; }
    public DateOnly? CheckIn { get; set; }
    public DateOnly? CheckOut { get; set; }
}