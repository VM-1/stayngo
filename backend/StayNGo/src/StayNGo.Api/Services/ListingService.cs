using Microsoft.EntityFrameworkCore;
using StayNGo.Api.Exceptions;
using StayNGo.Api.Extensions;
using StayNGo.Api.Features.Common;
using StayNGo.Api.Features.Listings;
using StayNGo.Api.Features.Listings.Get;
using StayNGo.Api.Features.Listings.ListMine;
using StayNGo.Api.Features.Listings.Update;
using StayNGo.Api.Services.Interfaces;
using StayNGo.Api.Utils;
using StayNGo.Domain.Entities;
using StayNGo.Domain.Enums;
using StayNGo.Infrastructure.Persistence;

namespace StayNGo.Api.Services;

public class ListingService(StayNGoDbContext db, ICurrentUserService currentUserService) : IListingService
{
    public async Task<ListingContract> GetListingAsync(Guid listingId)
    {
        var listing = await db.Listings
            .Where(x => x.Status == ListingStatus.Published)
            .Where(x => x.Id == listingId)
            .SingleOrDefaultAsync();

        if (listing is null)
        {
            throw new RecordNotFoundException(nameof(Listing), listingId);
        }

        return ListingContract.From(listing);
    }

    public async Task<PageResult<ListingContract>> GetListings(GetListingFilter filter)
    {
        var minPriceCents = MoneyConvertor.FromMajorUnits(filter.MinPrice);
        var maxPriceCents = MoneyConvertor.FromMajorUnits(filter.MaxPrice);

        IQueryable<Listing> query = db.Listings;

        if (filter is { CheckIn: not null, CheckOut: not null })
        {
            query = query.Where(x => !x.Bookings.Any(b => b.Status == BookingStatus.Confirmed &&
                                                          b.CheckIn < filter.CheckOut.Value &&
                                                          filter.CheckIn.Value < b.CheckOut));
        }

        query = query.Where(x => x.Status == ListingStatus.Published);

        if (!string.IsNullOrWhiteSpace(filter.Location))
        {
            query = query.Where(x => x.Location == filter.Location);
        }

        if (filter.MinCapacity.HasValue)
        {
            query = query.Where(x => x.Capacity >= filter.MinCapacity);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(x => x.Price!.AmountCents <= maxPriceCents);
        }

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(x => x.Price!.AmountCents >= minPriceCents);
        }

        if (!string.IsNullOrWhiteSpace(filter.Currency))
        {
            query = query.Where(x => x.Price!.Currency == filter.Currency);
        }

        var totalCount = await query.CountAsync();
        var listings = await query.OrderBy(x => x.CreatedAt).ApplyPagination(filter).ToListAsync();

        return new PageResult<ListingContract>(
            ListingContract.From(listings), filter.Page, filter.PageSize, totalCount);
    }

    public async Task<PageResult<ListingContract>> GetCurrentUserOwnListingsAsync(GetMyListingsFilter filter)
    {
        var user = await currentUserService.GetOrProvisionAsync();

        var query = db.Listings
            .AsNoTracking()
            .Where(x => x.OwnerUserId == user.Id)
            .OrderBy(x => x.CreatedAt)
            .AsQueryable();

        var totalCount = await query.CountAsync();
        query = query.ApplyPagination(filter);
        var listings = await query.ToListAsync();

        var result =
            new PageResult<ListingContract>(ListingContract.From(listings), filter.Page, filter.PageSize, totalCount);

        return result;
    }

    public async Task<ListingContract> CreateAsync(UpsertListingRequest model)
    {
        var user = await currentUserService.GetOrProvisionAsync();

        var listing = Listing.StartDraft(user.Id);

        listing.UpdateDraftDetails(model.Title, model.Description,
            model.Location, model.TimeZone,
            model.MainImageUrl, model.ImageUrls,
            model.Price?.ToDomain(), model.Capacity);

        db.Listings.Add(listing);
        await db.SaveChangesAsync();

        return ListingContract.From(listing);
    }

    public async Task<ListingContract> UpdateDraftListing(Guid listingId, UpsertListingRequest model)
    {
        var listing = await GetListingForCurrentUser(listingId);

        listing.UpdateDraftDetails(model.Title, model.Description,
            model.Location, model.TimeZone,
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