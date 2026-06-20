using StayNGo.Api.Features.Listings.Update;
using StayNGo.Api.Services.Interfaces;

namespace StayNGo.Api.Features.Listings.Create;

public class CreateListingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var groups = app.MapHostListingsGroup();

        groups.MapPost("", CreateListing);
    }

    private static async Task<IResult> CreateListing(IListingService listingService, UpsertListingRequest request)
    {
        return Results.Ok(await listingService.CreateAsync(request));

    }
}