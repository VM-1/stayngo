using StayNGo.Api.Features.Common;

namespace StayNGo.Api.Features.Listings.Update;

public record UpsertListingRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }  
    public List<string> ImageUrls { get; set; } = [];
    public string? MainImageUrl { get; set; }
    public string? Location { get; set; }
    public string? TimeZoneId { get; set; } 
    public MoneyContract? Price { get; set; }
    public int? Capacity { get; set; }
} 