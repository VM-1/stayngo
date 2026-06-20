using StayNGo.Api.Features.Common;
using StayNGo.Api.Features.Listings;
using StayNGo.Api.Features.Listings.Get;
using StayNGo.Api.Features.Listings.ListMine;
using StayNGo.Api.Features.Listings.Update;

namespace StayNGo.Api.Services.Interfaces;

public interface IListingService
{
    Task<ListingContract> CreateAsync(UpsertListingRequest model);
    Task<PageResult<ListingContract>> GetCurrentUserOwnListingsAsync(GetMyListingsFilter filter);
    Task<ListingContract> UpdatePublishedListing(Guid listingId, UpdatePublishedListingRequest model);
    Task<ListingContract> UpdateDraftListing(Guid listingId, UpsertListingRequest model);
    Task<ListingContract> Archive(Guid listingId);
    Task<ListingContract> PublishListing(Guid listingId);
    Task<PageResult<ListingContract>> GetListings(GetListingFilter filter);
    Task<ListingContract> GetListingAsync(Guid listingId);
}