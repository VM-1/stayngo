using FluentAssertions;
using StayNGo.Domain.Enums;
using StayNGo.Domain.Exceptions;

namespace StayNGo.IntegrationTests.Bookings;

public class BookingExclusionConstraintTests(IntegrationTestFactory factory) : BaseIntegrationTests(factory)
{
    [Fact]
    public async Task Overlapping_confirmed_bookings_are_rejected()
    {
        var host = await SeedUserAsync($"host-{Guid.NewGuid():N}@stayngo.com", "host");
        var guest = await SeedUserAsync($"guest-{Guid.NewGuid():N}@stayngo.com", "guest");
        var listing = await SeedPublishedListingAsync(host);

        await SeedBookingAsync(listing, guest,
            new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 5), BookingStatus.Confirmed);

        // A second confirmed booking that overlaps the first → GiST exclusion constraint (23P01),
        // surfaced as a DomainException by the DbContext's SaveChanges handler.
        var act = async () => await SeedBookingAsync(listing, guest,
            new DateOnly(2026, 7, 3), new DateOnly(2026, 7, 7), BookingStatus.Confirmed);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Back_to_back_confirmed_bookings_are_allowed()
    {
        var host = await SeedUserAsync($"host-{Guid.NewGuid():N}@stayngo.com", "host");
        var guest = await SeedUserAsync($"guest-{Guid.NewGuid():N}@stayngo.com", "guest");
        var listing = await SeedPublishedListingAsync(host);

        await SeedBookingAsync(listing, guest,
            new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 5), BookingStatus.Confirmed);

        // Check-out day == next check-in day: half-open ranges do not overlap.
        var act = async () => await SeedBookingAsync(listing, guest,
            new DateOnly(2026, 7, 5), new DateOnly(2026, 7, 8), BookingStatus.Confirmed);

        await act.Should().NotThrowAsync();
    }
}
