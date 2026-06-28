using Microsoft.AspNetCore.Http.HttpResults;
using StayNGo.Api.Features.Common;
using StayNGo.Api.Services.Interfaces;

namespace StayNGo.Api.Features.Listings.Update;

public class UpdateListingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var groups = app.MapHostListingsGroup();

        groups.MapPut("draft/{id:guid}", UpdateDraftListing).WithValidation<UpsertListingRequest>();
        groups.MapPatch("published/{id:guid}", UpdatePublishedListing).WithValidation<UpdatePublishedListingRequest>();
        groups.MapPut("archive/{id:guid}", ArchiveListing);
        groups.MapPut("publish/{id:guid}", PublishListing);
    }

    private static async Task<Ok<ListingContract>> PublishListing(IListingService service, Guid id)
    {
        return TypedResults.Ok(await service.PublishListing(id));
    }

    private static async Task<Ok<ListingContract>> ArchiveListing(IListingService service, Guid id)
    {
        return TypedResults.Ok(await service.Archive(id));
    }

    private static async Task<Ok<ListingContract>> UpdateDraftListing(IListingService service,
        UpsertListingRequest request, Guid id)
    {
        return TypedResults.Ok(await service.UpdateDraftListing(id, request));
    }

    private static async Task<Ok<ListingContract>> UpdatePublishedListing(IListingService service,
        UpdatePublishedListingRequest request, Guid id)
    {
        return TypedResults.Ok(await service.UpdatePublishedListing(id, request));
    }
}
