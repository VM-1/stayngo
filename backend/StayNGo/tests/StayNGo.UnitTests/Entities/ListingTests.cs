using FluentAssertions;
using StayNGo.Domain.Entities;
using StayNGo.Domain.Enums;
using StayNGo.Domain.Exceptions;
using StayNGo.Domain.ValueObjects;

namespace StayNGo.UnitTests.Entities;

public class ListingTests
{
    private static readonly Guid OwnerId = Guid.NewGuid();

    private sealed class DraftSpec
    {
        public string? Title = "Loft";
        public string? Description = "desc";
        public string? Location = "Lisbon";
        public string? TimeZoneId = "Europe/Lisbon";
        public string? MainImageUrl = "https://img/m.jpg";
        public List<string> ImageUrls = ["https://img/m.jpg"];
        public Money? Price = new(12000, "EUR");
        public int? Capacity = 4;

        public Listing Build(Guid? ownerId = null)
        {
            var l = Listing.StartDraft(ownerId ?? Guid.NewGuid());
            l.UpdateDraftDetails(Title, Description, Location, TimeZoneId,
                MainImageUrl, ImageUrls, Price, Capacity);
            return l;
        }

        public static Listing CompleteDraft()
        {
            return new DraftSpec().Build(OwnerId);
        }
    }

    [Fact]
    public void StartDraft_Always_CreatesDraftOwnedByUser()
    {
        var listing = Listing.StartDraft(OwnerId);

        listing.Status.Should().Be(ListingStatus.Draft);
        listing.OwnerUserId.Should().Be(OwnerId);
        listing.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void UpdateDraftDetails_OnDraft_SetsFields()
    {
        var listing = DraftSpec.CompleteDraft();

        listing.Title.Should().Be("Loft");
        listing.Price.Should().Be(new Money(12000, "EUR"));
        listing.Capacity.Should().Be(4);
    }

    [Fact]
    public void Publish_WhenAllFieldsPresent_TransitionsToPublished()
    {
        var listing = DraftSpec.CompleteDraft();

        listing.Publish();

        listing.Status.Should().Be(ListingStatus.Published);
    }

    [Fact]
    public void Publish_WhenAlreadyPublished_Throws()
    {
        var listing = DraftSpec.CompleteDraft();
        listing.Publish();

        var act = () => listing.Publish();

        act.Should().Throw<DomainException>()
            .WithMessage("*draft*");
    }

    [Fact]
    public void UpdateDraftDetails_WhenPublished_Throws()
    {
        var listing = DraftSpec.CompleteDraft();
        listing.Publish();

        var act = () => listing.UpdateDraftDetails("new", "new", "new", "Europe/Lisbon",
            "https://img/x.jpg", ["https://img/x.jpg"], new Money(1, "EUR"), 2);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void UpdatePublishedDetails_OnPublished_UpdatesAllowedFieldsOnly()
    {
        var listing = DraftSpec.CompleteDraft();
        listing.Publish();

        listing.UpdatePublishedDetails(
            description: "Updated copy",
            mainImageUrl: "https://img/new.jpg",
            imageUrls: ["https://img/new.jpg"],
            price: new Money(15000, "EUR"));

        listing.Description.Should().Be("Updated copy");
        listing.Price.Should().Be(new Money(15000, "EUR"));
        listing.Title.Should().Be("Loft"); // locked field unchanged
    }

    [Fact]
    public void UpdatePublishedDetails_OnDraft_Throws()
    {
        var listing = DraftSpec.CompleteDraft(); // still a draft

        var act = () => listing.UpdatePublishedDetails("d", "https://img/x.jpg",
            ["https://img/x.jpg"], new Money(1, "EUR"));

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Archive_TransitionsToArchived()
    {
        var listing = DraftSpec.CompleteDraft();
        listing.Publish();

        listing.Archive();

        listing.Status.Should().Be(ListingStatus.Archived);
    }

    [Fact]
    public void Archive_WhenAlreadyArchived_Throws()
    {
        var listing = DraftSpec.CompleteDraft();
        listing.Publish();
        listing.Archive();

        var act = () => listing.Archive();

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(ListingStatus.Published)]
    [InlineData(ListingStatus.Archived)]
    public void UpdateDraftDetails_WhenNotDraft_Throws(ListingStatus reachedStatus)
    {
        var listing = DraftSpec.CompleteDraft();
        listing.Publish();
        if (reachedStatus == ListingStatus.Archived)
            listing.Archive();

        var act = () => listing.UpdateDraftDetails("x", "x", "x", "Europe/Lisbon",
            "https://img/x.jpg", ["https://img/x.jpg"], new Money(1, "EUR"), 1);

        act.Should().Throw<DomainException>();
    }


    [Theory]
    [MemberData(nameof(IncompleteDrafts))]
    public void Publish_WhenFieldMissing_ReportsThatField(Listing draft, string expected)
    {
        var act = draft.Publish;
        act.Should().Throw<DomainException>().WithMessage($"*{expected}*");
    }

    public static IEnumerable<object[]> IncompleteDrafts() =>
    [
        [new DraftSpec { Title = string.Empty }.Build(), nameof(Listing.Title)],
        [new DraftSpec { Description = string.Empty }.Build(), nameof(Listing.Description)],
        [new DraftSpec { MainImageUrl = string.Empty }.Build(), nameof(Listing.MainImageUrl)],
        [new DraftSpec { TimeZoneId = string.Empty }.Build(), nameof(Listing.TimeZoneId)],
        [new DraftSpec { Location = string.Empty }.Build(), nameof(Listing.Location)],
        [new DraftSpec { Price = null }.Build(), nameof(Listing.Price)],
        [new DraftSpec { ImageUrls = [] }.Build(), nameof(Listing.ImageUrls)],
        [new DraftSpec { Capacity = null }.Build(), nameof(Listing.Capacity)]
    ];
}