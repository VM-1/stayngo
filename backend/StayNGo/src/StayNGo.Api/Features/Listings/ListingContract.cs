using StayNGo.Api.Features.Common;
using StayNGo.Domain.Entities;
using StayNGo.Domain.Enums;

namespace StayNGo.Api.Features.Listings;

public record ListingContract
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime? UpdatedAt { get; init; }

    public required string? Title { get; init; }
    public required string? Description { get; init; }
    public required List<string> ImageUrls { get; init; } = [];
    public required string? MainImageUrl { get; init; }
    public required string? Location { get; init; }
    public required string? TimeZoneId { get; init; }
    public required ListingStatus Status { get; init; }
    public required MoneyContract? Price { get; init; }
    public required int? Capacity { get; init; }


    public static ListingContract From(Listing listing)
    {
        return new ListingContract
        {
            Id = listing.Id,
            CreatedAt = listing.CreatedAt,
            UpdatedAt = listing.UpdatedAt,
            Title = listing.Title,
            Description = listing.Description,
            ImageUrls = listing.ImageUrls,
            MainImageUrl = listing.MainImageUrl,
            Location = listing.Location,
            TimeZoneId = listing.TimeZoneId?.ToString(),
            Status = listing.Status,
            Price = MoneyContract.From(listing.Price),
            Capacity = listing.Capacity
        };
    }
    public static List<ListingContract> From(IEnumerable<Listing> listings)
    {
        return listings.Select(From).ToList();
    }
}