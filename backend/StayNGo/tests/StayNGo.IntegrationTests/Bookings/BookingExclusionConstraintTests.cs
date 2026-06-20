using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using StayNGo.Domain.Entities;
using StayNGo.Domain.Enums;
using StayNGo.Domain.ValueObjects;

namespace StayNGo.IntegrationTests.Bookings;

public class BookingExclusionConstraintTests(IntegrationTestFactory factory) : BaseIntegrationTests(factory)
{
    [Fact]
    public async Task Overlapping_confirmed_bookings_are_rejected()
    {
        // Arrange
        var host = await SeedUserAsync("test@stayngo.com", "test name");
        var guest = await SeedUserAsync("testguest@stayngo.com", "test guest name");
        var listing = await SeedListingAsync(host);

        var firstBooking = await SeedBookingAsync(
            listing,
            guest,
            new DateRange(new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 5)),
            BookingStatus.Confirmed);

        // Act
        // Add Second Booking that has overlaps
        DbContext.Bookings.Add(new Booking
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            ListingId = listing.Id,
            GuestUserId = guest.Id,
            During = new DateRange(new DateOnly(2026, 7, 3), new DateOnly(2026, 7, 7)),
            TotalPrice = new Money(20000, "USD"),
            Status = BookingStatus.Confirmed,
        });

        var act = async () => await DbContext.SaveChangesAsync();

        // Assert - Postgres raised 23P01 (exclusion_violation)
        var ex = await act.Should().ThrowExactlyAsync<DbUpdateException>();
        ex.WithInnerException<PostgresException>().Which.SqlState.Should().Be("23P01");
    }


    // Helpers 
    private async Task<User> SeedUserAsync(string email, string displayName)
    {
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            DisplayName = displayName,
            ClerkId = Guid.NewGuid().ToString(),
        };

        DbContext.Users.Add(newUser);
        await DbContext.SaveChangesAsync();
        return newUser;
    }

    private async Task<Listing> SeedListingAsync(User owner)
    {
        var newListing = Listing.StartDraft(owner.Id);
        newListing.UpdateDraftDetails(title: "Test Listing", description: "For exclusion-constraint tests",
            location: "Test Location", timeZoneId: "America/New_York",
            price: new Money(1000, "USD"), capacity: 4,
            mainImageUrl: "https://placeholder.local/main.jpg", imageUrls: []);

        DbContext.Listings.Add(newListing);
        await DbContext.SaveChangesAsync();
        return newListing;
    }

    private async Task<Booking> SeedBookingAsync(
        Listing listing, User guest, DateRange during, BookingStatus status)
    {
        var b = new Booking
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            ListingId = listing.Id,
            GuestUserId = guest.Id,
            During = during,
            TotalPrice = new Money(20000, "USD"),
            Status = status,
        };
        DbContext.Bookings.Add(b);
        await DbContext.SaveChangesAsync();
        return b;
    }
}