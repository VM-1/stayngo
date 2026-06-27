using Microsoft.AspNetCore.Http.HttpResults;
using StayNGo.Api.Features.Common;
using StayNGo.Api.Services.Interfaces;

namespace StayNGo.Api.Features.Listings.ListMine;

public class GetMyListingsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var groups = app.MapHostListingsGroup();

        groups.MapGet("", GetMyListings);
    }

    private static async Task<Ok<PageResult<ListingContract>>> GetMyListings(IListingService service,
        [AsParameters] GetMyListingsFilter filter)
    {
        return TypedResults.Ok(await service.GetCurrentUserOwnListingsAsync(filter));
    }
}
