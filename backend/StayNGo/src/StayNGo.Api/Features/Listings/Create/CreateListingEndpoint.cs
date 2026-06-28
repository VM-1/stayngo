using Microsoft.AspNetCore.Http.HttpResults;
using StayNGo.Api.Features.Common;
using StayNGo.Api.Features.Listings.Update;
using StayNGo.Api.Services.Interfaces;

namespace StayNGo.Api.Features.Listings.Create;

public class CreateListingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var groups = app.MapHostListingsGroup();

        groups.MapPost("", CreateListing).WithValidation<UpsertListingRequest>();
    }

    private static async Task<Ok<ListingContract>> CreateListing(IListingService service, UpsertListingRequest request)
    {
        return TypedResults.Ok(await service.CreateAsync(request));
    }
}
