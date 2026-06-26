using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StayNGo.Api.Exceptions;
using StayNGo.Api.Features.Bookings;
using StayNGo.Api.Features.Bookings.Create;
using StayNGo.Api.Services.Interfaces;
using StayNGo.Domain.Enums;
using StayNGo.Domain.Exceptions;

namespace StayNGo.IntegrationTests.Bookings;

public class BookingServiceTests(IntegrationTestFactory factory) : BaseIntegrationTests(factory)
{
    private IBookingService Service => Scope.ServiceProvider.GetRequiredService<IBookingService>();

    private static readonly DateOnly Jul1 = new(2026, 7, 1);
    private static readonly DateOnly Jul5 = new(2026, 7, 5);
    private static readonly DateOnly Jul3 = new(2026, 7, 3);
    private static readonly DateOnly Jul7 = new(2026, 7, 7);
    private static readonly DateOnly Aug1 = new(2026, 8, 1);
    private static readonly DateOnly Aug5 = new(2026, 8, 5);

    // ---- Create ----

    [Fact]
    public async Task CreateAsync_CreatesPendingBooking_WithPriceFixedAtNightsTimesNightly()
    {
        var host = await SeedUserAsync($"host-{Guid.NewGuid():N}@it.test", "host");
        var listing = await SeedPublishedListingAsync(host); // nightly = 10000

        var result = await Service.CreateAsync(new CreateBookingRequest(listing.Id, Jul1, Jul5), Guid.NewGuid());

        result.Status.Should().Be(BookingStatus.Pending);
        result.TotalPrice.AmountCents.Should().Be(10000 * 4); // 4 nights
    }

    [Fact]
    public async Task CreateAsync_OnOwnListing_ThrowsNotFound()
    {
        var listing = await SeedPublishedListingAsync(Factory.PrimaryUser); // current user owns it

        var act = () => Service.CreateAsync(new CreateBookingRequest(listing.Id, Jul1, Jul5), Guid.NewGuid());

        await act.Should().ThrowAsync<RecordNotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_DuplicateIdempotencyKey_ReturnsSameBookingAndCreatesOne()
    {
        var host = await SeedUserAsync($"host-{Guid.NewGuid():N}@it.test", "host");
        var listing = await SeedPublishedListingAsync(host);
        var key = Guid.NewGuid();

        var first = await Service.CreateAsync(new CreateBookingRequest(listing.Id, Jul1, Jul5), key);
        var second = await Service.CreateAsync(new CreateBookingRequest(listing.Id, Jul1, Jul5), key);

        second.Id.Should().Be(first.Id);
        (await DbContext.Bookings.CountAsync(x => x.IdempotencyKey == key)).Should().Be(1);
    }

    // ---- Confirm / Reject ----

    [Fact]
    public async Task ConfirmReservation_WhenPending_SetsConfirmed()
    {
        var guest = await SeedUserAsync($"guest-{Guid.NewGuid():N}@it.test", "guest");
        var listing = await SeedPublishedListingAsync(Factory.PrimaryUser); // current user is the host
        var booking = await SeedBookingAsync(listing, guest, Jul1, Jul5, BookingStatus.Pending);

        var result = await Service.ConfirmReservation(booking.Id);

        result.Status.Should().Be(BookingStatus.Confirmed);
    }

    [Fact]
    public async Task ConfirmReservation_ByNonOwner_ThrowsNotFound()
    {
        var otherHost = await SeedUserAsync($"host-{Guid.NewGuid():N}@it.test", "host");
        var guest = await SeedUserAsync($"guest-{Guid.NewGuid():N}@it.test", "guest");
        var listing = await SeedPublishedListingAsync(otherHost); // NOT the current user
        var booking = await SeedBookingAsync(listing, guest, Jul1, Jul5, BookingStatus.Pending);

        var act = () => Service.ConfirmReservation(booking.Id);

        await act.Should().ThrowAsync<RecordNotFoundException>();
    }

    [Fact]
    public async Task RejectReservation_WhenPending_SetsRejected()
    {
        var guest = await SeedUserAsync($"guest-{Guid.NewGuid():N}@it.test", "guest");
        var listing = await SeedPublishedListingAsync(Factory.PrimaryUser);
        var booking = await SeedBookingAsync(listing, guest, Jul1, Jul5, BookingStatus.Pending);

        var result = await Service.RejectReservation(booking.Id);

        result.Status.Should().Be(BookingStatus.Rejected);
    }

    [Fact]
    public async Task ConfirmReservation_WhenOverlapsAnotherConfirmed_Throws()
    {
        var guest1 = await SeedUserAsync($"guest-{Guid.NewGuid():N}@it.test", "guest1");
        var guest2 = await SeedUserAsync($"guest-{Guid.NewGuid():N}@it.test", "guest2");
        var listing = await SeedPublishedListingAsync(Factory.PrimaryUser);
        var first = await SeedBookingAsync(listing, guest1, Jul1, Jul5, BookingStatus.Pending);
        var second = await SeedBookingAsync(listing, guest2, Jul3, Jul7, BookingStatus.Pending);

        await Service.ConfirmReservation(first.Id);

        // Second overlaps the now-confirmed first → exclusion constraint → DomainException.
        var act = () => Service.ConfirmReservation(second.Id);

        await act.Should().ThrowAsync<DomainException>();
    }

    // ---- Reads ----

    [Fact]
    public async Task GetMyTrips_ReturnsOnlyCurrentUsersBookings()
    {
        await ClearBookingsAsync();
        var host = await SeedUserAsync($"host-{Guid.NewGuid():N}@it.test", "host");
        var listing = await SeedPublishedListingAsync(host);

        var mine = await Service.CreateAsync(new CreateBookingRequest(listing.Id, Jul1, Jul5), Guid.NewGuid());
        var otherGuest = await SeedUserAsync($"guest-{Guid.NewGuid():N}@it.test", "other");
        await SeedBookingAsync(listing, otherGuest, Aug1, Aug5, BookingStatus.Pending);

        var result = await Service.GetMyTrips(new GetBookingFilter());

        result.Items.Should().ContainSingle().Which.Id.Should().Be(mine.Id);
    }

    [Fact]
    public async Task GetReservations_ReturnsOwnedListingBookings_PendingFirst()
    {
        await ClearBookingsAsync();
        var guest = await SeedUserAsync($"guest-{Guid.NewGuid():N}@it.test", "guest");
        var listing = await SeedPublishedListingAsync(Factory.PrimaryUser);

        // Pending seeded first (older); confirmed second (newer). Pending must still sort to the top.
        var pending = await SeedBookingAsync(listing, guest, Aug1, Aug5, BookingStatus.Pending);
        await SeedBookingAsync(listing, guest, Jul1, Jul5, BookingStatus.Confirmed);

        DbContext.ChangeTracker.Clear(); // force a real read so the Guest Include is actually exercised

        var result = await Service.GetReservations(new GetBookingFilter());

        result.Items.Should().HaveCount(2);
        result.Items.First().Id.Should().Be(pending.Id);
    }

    // ---- Cancel ----

    [Fact]
    public async Task CancelTrip_OwnBookingBeforeCutoff_Cancels()
    {
        var host = await SeedUserAsync($"host-{Guid.NewGuid():N}@it.test", "host");
        var listing = await SeedPublishedListingAsync(host);
        // Far-future check-in → well before the 24h cutoff for the real "now" the service uses.
        var future = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(1);
        var booking = await Service.CreateAsync(
            new CreateBookingRequest(listing.Id, future, future.AddDays(2)), Guid.NewGuid());

        var result = await Service.CancelTrip(booking.Id);

        result.Status.Should().Be(BookingStatus.Cancelled);
    }

    [Fact]
    public async Task CancelTrip_ForeignBooking_ThrowsNotFound()
    {
        var host = await SeedUserAsync($"host-{Guid.NewGuid():N}@it.test", "host");
        var otherGuest = await SeedUserAsync($"guest-{Guid.NewGuid():N}@it.test", "other");
        var listing = await SeedPublishedListingAsync(host);
        var future = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(1);
        var foreign = await SeedBookingAsync(listing, otherGuest, future, future.AddDays(2), BookingStatus.Pending);

        var act = () => Service.CancelTrip(foreign.Id); // current user is not the guest

        await act.Should().ThrowAsync<RecordNotFoundException>();
    }

    private async Task ClearBookingsAsync()
    {
        DbContext.Bookings.RemoveRange(DbContext.Bookings);
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear();
    }
}
