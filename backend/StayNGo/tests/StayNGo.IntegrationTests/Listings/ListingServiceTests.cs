using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StayNGo.Api.Exceptions;
using StayNGo.Api.Features.Common;
using StayNGo.Api.Features.Listings.ListMine;
using StayNGo.Api.Features.Listings.Update;
using StayNGo.Api.Services.Interfaces;
using StayNGo.Domain.Entities;
using StayNGo.Domain.Enums;
using StayNGo.Domain.Exceptions;

namespace StayNGo.IntegrationTests.Listings;

public class ListingServiceTests(IntegrationTestFactory factory) : BaseIntegrationTests(factory)
{
    private IListingService Service => Scope.ServiceProvider.GetRequiredService<IListingService>();

    [Fact]
    public async Task UpdateDraftListing_OnOtherUsersListing_ThrowsNotFound()
    {
        var foreign = await SeedForeignDraftAsync();

        var act = () => Service.UpdateDraftListing(foreign.Id, ValidRequest());

        await act.Should().ThrowAsync<RecordNotFoundException>();
    }

    [Fact]
    public async Task PublishListing_WhenDraftIncomplete_ThrowsDomainException()
    {
        var incomplete = ValidRequest() with { Price = null };
        var draft = await Service.CreateAsync(incomplete);

        var act = () => Service.PublishListing(draft.Id);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Price*");
    }

    [Fact]
    public async Task PublishListing_WhenDraftComplete_SetsPublished()
    {
        var draft = await Service.CreateAsync(ValidRequest());

        var result = await Service.PublishListing(draft.Id);

        result.Status.Should().Be(ListingStatus.Published);
    }

    [Fact]
    public async Task Archive_WhenPublished_SetsArchived()
    {
        var draft = await Service.CreateAsync(ValidRequest());
        await Service.PublishListing(draft.Id);

        var result = await Service.Archive(draft.Id);

        result.Status.Should().Be(ListingStatus.Archived);
    }

    [Fact]
    public async Task GetMine_ReturnsOnlyCurrentUsersListings()
    {
        DbContext.Listings.RemoveRange(DbContext.Listings);
        await DbContext.SaveChangesAsync();

        var otherUser = new User
        {
            Id = Guid.CreateVersion7(),
            ClerkId = "other-clerk",
            Email = "other@integration.test",
            DisplayName = "Other",
        };
        var foreign = Listing.StartDraft(otherUser.Id);
        DbContext.Users.Add(otherUser);
        DbContext.Listings.Add(foreign);
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear();

        var service = Scope.ServiceProvider.GetRequiredService<IListingService>();

        var mine = await service.CreateAsync(ValidRequest());

        var result = await service.GetCurrentUserOwnListingsAsync(new GetMyListingsFilter());

        result.Should().ContainSingle()
            .Which.Id.Should().Be(mine.Id);
        result.Should().NotContain(x => x.Id == foreign.Id);
    }

    [Fact]
    public async Task CreateAsync_PersistsDraftOwnedByCurrentUser()
    {
        var listingService = Scope.ServiceProvider.GetRequiredService<IListingService>();

        var result = await listingService.CreateAsync(ValidRequest());

        result.Status.Should().Be(ListingStatus.Draft);

        var dbListing = await DbContext.Listings.FindAsync(result.Id);
        dbListing.Should().NotBeNull();
        dbListing!.OwnerUserId.Should().Be(Factory.PrimaryUser.Id);
    }

    private async Task<Listing> SeedForeignDraftAsync()
    {
        var otherUser = new User
        {
            Id = Guid.CreateVersion7(),
            ClerkId = $"other-{Guid.NewGuid():N}",
            Email = $"other-{Guid.NewGuid():N}@integration.test",
            DisplayName = "Other",
        };
        var foreign = Listing.StartDraft(otherUser.Id);

        DbContext.Users.Add(otherUser);
        DbContext.Listings.Add(foreign);
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear();

        return foreign;
    }

    private static UpsertListingRequest ValidRequest() => new()
    {
        Title = "Sunlit loft",
        Description = "A bright loft in the old town.",
        Location = "Lisbon, Portugal",
        TimeZone = "Europe/Lisbon",
        MainImageUrl = "https://img/main.jpg",
        ImageUrls = ["https://img/main.jpg"],
        Price = new MoneyContract(12000, "EUR"),
        Capacity = 4,
    };
}