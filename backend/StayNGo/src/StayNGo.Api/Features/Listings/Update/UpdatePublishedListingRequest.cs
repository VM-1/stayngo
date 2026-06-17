using StayNGo.Api.Features.Common;

namespace StayNGo.Api.Features.Listings.Update;

public record UpdatePublishedListingRequest
{
    public required string Description { get; set; }   = null!;
    public required List<string> ImageUrls { get; set; } = [];
    public required  string MainImageUrl { get; set; } = null!;
    public required MoneyContract Price { get; set; } = null!;
}