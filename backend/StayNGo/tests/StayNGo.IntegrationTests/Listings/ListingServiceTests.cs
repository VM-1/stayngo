using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StayNGo.Api.Exceptions;
using StayNGo.Api.Features.Common;
using StayNGo.Api.Features.Listings;
using StayNGo.Api.Features.Listings.Get;
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

        result.Items.Should().ContainSingle().Which.Id.Should().Be(mine.Id);
        result.Items.Should().NotContain(x => x.Id == foreign.Id);
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

    [Fact]
    public async Task GetListings_ReturnsOnlyPublishedListings()
    {
        await ClearListingsAsync();
        var published = await SeedPublishedAsync();
        var draft = await Service.CreateAsync(ValidRequest()); // stays Draft

        var result = await Service.GetListings(new GetListingFilter());

        result.Items.Should().ContainSingle().Which.Id.Should().Be(published.Id);
        result.Items.Should().NotContain(x => x.Id == draft.Id);
    }

    [Fact]
    public async Task GetListings_FiltersByMaxPrice_ExcludesPricier()
    {
        await ClearListingsAsync();
        var cheap = await SeedPublishedAsync(priceCents: 8000);   // €80
        await SeedPublishedAsync(priceCents: 12000);              // €120

        // €100 ceiling → only the €80 listing
        var result = await Service.GetListings(new GetListingFilter { MaxPrice = 100m });

        result.Items.Should().ContainSingle().Which.Id.Should().Be(cheap.Id);
    }

    [Fact]
    public async Task GetListings_FiltersByMinPrice_ExcludesCheaper()
    {
        await ClearListingsAsync();
        await SeedPublishedAsync(priceCents: 8000);              // €80
        var pricey = await SeedPublishedAsync(priceCents: 12000); // €120

        var result = await Service.GetListings(new GetListingFilter { MinPrice = 100m });

        result.Items.Should().ContainSingle().Which.Id.Should().Be(pricey.Id);
    }

    [Fact]
    public async Task GetListings_FiltersByMinCapacity_IsInclusiveOfBoundary()
    {
        await ClearListingsAsync();
        await SeedPublishedAsync(capacity: 2);
        var exact = await SeedPublishedAsync(capacity: 4);

        // requesting 4 must include a place that sleeps exactly 4
        var result = await Service.GetListings(new GetListingFilter { MinCapacity = 4 });

        result.Items.Should().ContainSingle().Which.Id.Should().Be(exact.Id);
    }

    [Fact]
    public async Task GetListings_FiltersByLocation()
    {
        await ClearListingsAsync();
        var lisbon = await SeedPublishedAsync(location: "Lisbon, Portugal");
        await SeedPublishedAsync(location: "Porto, Portugal");

        var result = await Service.GetListings(new GetListingFilter { Location = "Lisbon, Portugal" });

        result.Items.Should().ContainSingle().Which.Id.Should().Be(lisbon.Id);
    }

    [Fact]
    public async Task GetListings_Paginates_ReportsTotalAndPageSize()
    {
        await ClearListingsAsync();
        await SeedPublishedAsync();
        await SeedPublishedAsync();
        await SeedPublishedAsync();

        var result = await Service.GetListings(new GetListingFilter { Page = 1, PageSize = 2 });

        result.Items.Should().HaveCount(2);
        result.Total.Should().Be(3);
        result.PageSize.Should().Be(2);
    }

    [Fact]
    public async Task GetListingAsync_WhenPublished_ReturnsListing()
    {
        var published = await SeedPublishedAsync();

        var result = await Service.GetListingAsync(published.Id);

        result.Id.Should().Be(published.Id);
        result.Status.Should().Be(ListingStatus.Published);
    }

    [Fact]
    public async Task GetListingAsync_WhenNotPublished_ThrowsNotFound()
    {
        var draft = await Service.CreateAsync(ValidRequest()); // Draft, never published

        var act = () => Service.GetListingAsync(draft.Id);

        await act.Should().ThrowAsync<RecordNotFoundException>();
    }

    [Fact]
    public async Task GetListingAsync_WhenUnknownId_ThrowsNotFound()
    {
        var act = () => Service.GetListingAsync(Guid.CreateVersion7());

        await act.Should().ThrowAsync<RecordNotFoundException>();
    }

    private async Task ClearListingsAsync()
    {
        DbContext.Listings.RemoveRange(DbContext.Listings);
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear();
    }

    private async Task<ListingContract> SeedPublishedAsync(
        long priceCents = 12000, int capacity = 4, string location = "Lisbon, Portugal")
    {
        var draft = await Service.CreateAsync(ValidRequest() with
        {
            Price = new MoneyContract(priceCents, "EUR"),
            Capacity = capacity,
            Location = location,
        });
        return await Service.PublishListing(draft.Id);
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