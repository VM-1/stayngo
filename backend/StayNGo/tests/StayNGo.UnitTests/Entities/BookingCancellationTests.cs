using FluentAssertions;
using StayNGo.Domain.Entities;
using StayNGo.Domain.Enums;
using StayNGo.Domain.Exceptions;
using StayNGo.Domain.ValueObjects;

namespace StayNGo.UnitTests.Entities;

public class BookingCancellationTests
{
    private static readonly DateOnly CheckIn = new(2026, 7, 10);
    private static readonly DateOnly CheckOut = new(2026, 7, 12);

    // Cutoff is 15:00 (local to the listing) on the day before check-in: 2026-07-09 15:00.

    private static Listing PublishedListing(string timeZone)
    {
        var listing = Listing.StartDraft(Guid.NewGuid());
        listing.UpdateDraftDetails("Loft", "desc", "Lisbon", timeZone,
            "https://img/m.jpg", ["https://img/m.jpg"], new Money(12000, "EUR"), 4);
        listing.Publish();
        return listing;
    }

    private static Booking PendingBooking(Listing listing) =>
        Booking.CreateBooking(Guid.NewGuid(), CheckIn, CheckOut, listing, Guid.NewGuid());

    [Fact]
    public void Cancel_WellBeforeCutoff_WhenPending_SetsCancelled()
    {
        var listing = PublishedListing("Europe/Lisbon");
        var booking = PendingBooking(listing);
        var now = new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero);

        booking.Cancel(now, listing.TimeZone!);

        booking.Status.Should().Be(BookingStatus.Cancelled);
    }

    [Fact]
    public void Cancel_BeforeCutoff_WhenConfirmed_SetsCancelled()
    {
        var listing = PublishedListing("Europe/Lisbon");
        var booking = PendingBooking(listing);
        booking.Confirm();
        var now = new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero);

        booking.Cancel(now, listing.TimeZone!);

        booking.Status.Should().Be(BookingStatus.Cancelled);
    }

    [Fact]
    public void Cancel_AtCutoff_Throws()
    {
        var listing = PublishedListing("Europe/Lisbon");
        var booking = PendingBooking(listing);
        // 2026-07-09 15:00 Lisbon (UTC+1 in July) == 14:00 UTC — exactly the cutoff.
        var now = new DateTimeOffset(2026, 7, 9, 14, 0, 0, TimeSpan.Zero);

        var act = () => booking.Cancel(now, listing.TimeZone!);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Cancel_AfterCheckIn_Throws()
    {
        var listing = PublishedListing("Europe/Lisbon");
        var booking = PendingBooking(listing);
        var now = new DateTimeOffset(2026, 7, 11, 8, 0, 0, TimeSpan.Zero);

        var act = () => booking.Cancel(now, listing.TimeZone!);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_Throws()
    {
        var listing = PublishedListing("Europe/Lisbon");
        var booking = PendingBooking(listing);
        var now = new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero);
        booking.Cancel(now, listing.TimeZone!);

        var act = () => booking.Cancel(now, listing.TimeZone!);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Cancel_CutoffIsEvaluatedInListingTimeZone()
    {
        // Same instant (2026-07-09 16:00 UTC), two listings in different zones.
        var now = new DateTimeOffset(2026, 7, 9, 16, 0, 0, TimeSpan.Zero);

        // Lisbon (UTC+1): local 17:00 — past the 15:00 cutoff → rejected.
        var lisbonListing = PublishedListing("Europe/Lisbon");
        var lisbon = PendingBooking(lisbonListing);
        var actLisbon = () => lisbon.Cancel(now, lisbonListing.TimeZone!);
        actLisbon.Should().Throw<DomainException>();

        // Honolulu (UTC-10): local 06:00 — before the 15:00 cutoff → allowed.
        var honoluluListing = PublishedListing("Pacific/Honolulu");
        var honolulu = PendingBooking(honoluluListing);
        honolulu.Cancel(now, honoluluListing.TimeZone!);
        honolulu.Status.Should().Be(BookingStatus.Cancelled);
    }
}
