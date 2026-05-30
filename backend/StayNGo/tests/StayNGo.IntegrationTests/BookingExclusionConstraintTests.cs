using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using StayNGo.Domain.Entities;
using StayNGo.Domain.Enums;
using StayNGo.Domain.ValueObjects;
using StayNGo.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace StayNGo.IntegrationTests;

public class BookingExclusionConstraintTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _pg = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .Build();

    private StayNGoDbContext _db = null!;

    public async Task InitializeAsync()
    {
        await _pg.StartAsync();
        var options = new DbContextOptionsBuilder<StayNGoDbContext>()
            .UseNpgsql(_pg.GetConnectionString())
            .UseSnakeCaseNamingConvention()
            .Options;

        _db = new StayNGoDbContext(options);

        await _db.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
        await _pg.DisposeAsync();
    }

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
        _db.Bookings.Add(new Booking
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            ListingId = listing.Id,
            GuestUserId = guest.Id,
            During = new DateRange(new DateOnly(2026, 7, 3), new DateOnly(2026, 7, 7)),
            TotalPrice = new Money(20000, "USD"),
            Status = BookingStatus.Confirmed,
        });

        var act = async () => await _db.SaveChangesAsync();
        
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

        _db.Users.Add(newUser);
        await _db.SaveChangesAsync();
        return newUser;
    }

    private async Task<Listing> SeedListingAsync(User owner)
    {
        var newListing = new Listing
        {
            Id = Guid.NewGuid(),
            OwnerUserId = owner.Id,
            Title = "Test Listing",
            Description = "For exclusion-constraint tests",
            ImageUrls = [],
            MainImageUrl = "https://placeholder.local/main.jpg",
            Location = "Test Location",
            TimeZoneId = "America/New_York",
            Capacity = 4,
            Status = ListingStatus.Published,
            Price = new Money(1000, "USD")
        };
        _db.Listings.Add(newListing);
        await _db.SaveChangesAsync();
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
        _db.Bookings.Add(b);
        await _db.SaveChangesAsync();
        return b;
    }
}