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

    private static async Task<IResult> PublishListing(IListingService service, Guid id)
    {
        return Results.Ok(await service.PublishListing(id));
    }
    
    private static async Task<IResult> ArchiveListing(IListingService service, Guid id)
    {
        return Results.Ok(await service.Archive(id));
    }
    private static async Task<IResult> UpdateDraftListing(IListingService service,
        UpsertListingRequest request, Guid id)
    {
        return Results.Ok(await service.UpdateDraftListing(id,request));
    }

    private static async Task<IResult> UpdatePublishedListing(IListingService service,
        UpdatePublishedListingRequest request, Guid id)
    {
        return Results.Ok(await service.UpdatePublishedListing(id,request));
    }
}