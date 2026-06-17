using Microsoft.EntityFrameworkCore;
using StayNGo.Api.Exceptions;
using StayNGo.Api.Extensions;
using StayNGo.Api.Features.Listings;
using StayNGo.Api.Features.Listings.ListMine;
using StayNGo.Api.Features.Listings.Update;
using StayNGo.Api.Services.Interfaces;
using StayNGo.Domain.Entities;
using StayNGo.Infrastructure.Persistence;

namespace StayNGo.Api.Services;

public class ListingService(StayNGoDbContext db, ICurrentUserService currentUserService) : IListingService
{
    public async Task<ListingContract> CreateAsync(UpsertListingRequest model)
    {
        var user = await currentUserService.GetOrProvisionAsync();

        var listing = Listing.StartDraft(user.Id);

        listing.UpdateDraftDetails(model.Title, model.Description,
            model.Location, model.TimeZoneId,
            model.MainImageUrl, model.ImageUrls,
            model.Price?.ToDomain(), model.Capacity);

        db.Listings.Add(listing);
        await db.SaveChangesAsync();

        return ListingContract.From(listing);
    }

    public async Task<List<ListingContract>> GetCurrentUserOwnListingsAsync(GetMyListingsFilter filter)
    {
        var user = await currentUserService.GetOrProvisionAsync();

        var query = db.Listings
            .AsNoTracking()
            .Where(x => x.OwnerUserId == user.Id)
            .ApplyPagination(filter);

        var listings = await query.ToListAsync();

        return ListingContract.From(listings);
    }

    public async Task<ListingContract> UpdateDraftListing(Guid listingId, UpsertListingRequest model)
    {
        var listing = await GetListingForCurrentUser(listingId);

        listing.UpdateDraftDetails(model.Title, model.Description,
            model.Location, model.TimeZoneId,
            model.MainImageUrl, model.ImageUrls,
            model.Price?.ToDomain(), model.Capacity);

        await db.SaveChangesAsync();

        return ListingContract.From(listing);
    }

    public async Task<ListingContract> UpdatePublishedListing(Guid listingId, UpdatePublishedListingRequest model)
    {
        var listing = await GetListingForCurrentUser(listingId);

        listing.UpdatePublishedDetails(model.Description, model.MainImageUrl, model.ImageUrls, model.Price.ToDomain());

        await db.SaveChangesAsync();

        return ListingContract.From(listing);
    }

    public async Task<ListingContract> PublishListing(Guid listingId)
    {
        var listing = await GetListingForCurrentUser(listingId);
        listing.Publish();
        await db.SaveChangesAsync();
        
        return ListingContract.From(listing);
    }

    public async Task<ListingContract> Archive(Guid listingId)
    {
        var listing = await GetListingForCurrentUser(listingId);
        listing.Archive();
        await db.SaveChangesAsync();
        
        return ListingContract.From(listing);
    }

    private async Task<Listing> GetListingForCurrentUser(Guid listingId)
    {
        var user = await currentUserService.GetOrProvisionAsync();
        var listing = await db.Listings.SingleOrDefaultAsync(x => x.Id == listingId && x.OwnerUserId == user.Id);
        if (listing is null)
        {
            throw new RecordNotFoundException(nameof(Listing), listingId);
        }

        return listing;
    }
}