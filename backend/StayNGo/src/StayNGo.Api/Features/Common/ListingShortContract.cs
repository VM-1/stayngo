using StayNGo.Domain.Entities;

namespace StayNGo.Api.Features.Common;

public record ListingShortContract
{
    public required Guid Id { get; init; }
    public required string? Title { get; init; }
    public required string? MainImageUrl { get; init; }

    public static ListingShortContract FromDomain(Listing listing)
    {
        return new ListingShortContract
        {
            Id = listing.Id,
            Title = listing.Title,
            MainImageUrl = listing.MainImageUrl,
        };
    }
}
