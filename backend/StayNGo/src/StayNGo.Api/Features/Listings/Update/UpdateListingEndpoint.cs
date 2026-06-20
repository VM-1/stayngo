using StayNGo.Api.Services.Interfaces;

namespace StayNGo.Api.Features.Listings.Update;

public class UpdateListingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var groups = app.MapHostListingsGroup();

        groups.MapPut("draft/{id:guid}", UpdateDraftListing);
        groups.MapPatch("published/{id:guid}", UpdatePublishedListing);
        groups.MapPut("archive/{id:guid}", ArchiveListing);
        groups.MapPut("publish/{id:guid}", PublishListing);
    }

    private static async Task<IResult> PublishListing(IListingService listingService, Guid id)
    {
        return Results.Ok(await listingService.PublishListing(id));
    }
    
    private static async Task<IResult> ArchiveListing(IListingService listingService, Guid id)
    {
        return Results.Ok(await listingService.Archive(id));
    }
    private static async Task<IResult> UpdateDraftListing(IListingService listingService,
        UpsertListingRequest request, Guid id)
    {
        return Results.Ok(await listingService.UpdateDraftListing(id,request));
    }

    private static async Task<IResult> UpdatePublishedListing(IListingService listingService,
        UpdatePublishedListingRequest request, Guid id)
    {
        return Results.Ok(await listingService.UpdatePublishedListing(id,request));
    }
}