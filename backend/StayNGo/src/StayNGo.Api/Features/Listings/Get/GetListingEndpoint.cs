using Microsoft.AspNetCore.Http.HttpResults;
using StayNGo.Api.Features.Common;
using StayNGo.Api.Services.Interfaces;

namespace StayNGo.Api.Features.Listings.Get;

public class GetListingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var groups = app.MapListingGroup();

        groups.MapGet("", GetListings);
        groups.MapGet("{id:guid}", GetListingById);
    }

    private static async Task<Ok<PageResult<ListingContract>>> GetListings(IListingService service,
        [AsParameters] GetListingFilter filter)
    {
        return TypedResults.Ok(await service.GetListings(filter));
    }

    private static async Task<Ok<ListingContract>> GetListingById(IListingService service, Guid id)
    {
        return TypedResults.Ok(await service.GetListingAsync(id));
    }
}
