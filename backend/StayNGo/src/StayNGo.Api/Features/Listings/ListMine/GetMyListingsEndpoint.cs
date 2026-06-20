using StayNGo.Api.Services.Interfaces;

namespace StayNGo.Api.Features.Listings.ListMine;

public class GetMyListingsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var groups = app.MapHostListingsGroup();

        groups.MapGet("", GetMyListings);
    }

    private static async Task<IResult> GetMyListings(IListingService listingService,
        [AsParameters] GetMyListingsFilter filter)
    {
        return Results.Ok(await listingService.GetCurrentUserOwnListingsAsync(filter));
    }
}