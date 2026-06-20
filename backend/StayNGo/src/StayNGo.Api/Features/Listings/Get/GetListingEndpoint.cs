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

    private static async Task<IResult> GetListings(IListingService listingService,
        [AsParameters] GetListingFilter filter)
    {
        return Results.Ok(await listingService.GetListings(filter));
    }

    private static async Task<IResult> GetListingById(IListingService listingService, Guid id)
    {
        return Results.Ok(await listingService.GetListingAsync(id));
    }
}