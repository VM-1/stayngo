using Microsoft.Extensions.DependencyInjection;
using StayNGo.Domain.Entities;
using StayNGo.Domain.Enums;
using StayNGo.Domain.ValueObjects;
using StayNGo.Infrastructure.Persistence;

namespace StayNGo.IntegrationTests;

public class BaseIntegrationTests : IClassFixture<IntegrationTestFactory>
{
    protected readonly IServiceScope Scope;
    protected readonly StayNGoDbContext DbContext;
    protected IntegrationTestFactory Factory { get; set; }

    protected BaseIntegrationTests(IntegrationTestFactory factory)
    {
        Factory = factory;
        Scope = factory.Services.CreateScope();

        DbContext = Scope.ServiceProvider.GetRequiredService<StayNGoDbContext>();
    }

    protected async Task<User> SeedUserAsync(string email, string displayName)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            DisplayName = displayName,
            ClerkId = Guid.NewGuid().ToString(),
        };

        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync();
        return user;
    }

    protected async Task<Listing> SeedPublishedListingAsync(User owner)
    {
        var listing = Listing.StartDraft(owner.Id);
        listing.UpdateDraftDetails(
            title: "Test Listing", description: "For booking tests",
            location: "Test City", timeZone: "America/New_York",
            mainImageUrl: "https://placeholder.local/main.jpg", imageUrls: ["https://placeholder.local/main.jpg"],
            price: new Money(10000, "USD"), capacity: 4);
        listing.Publish();

        DbContext.Listings.Add(listing);
        await DbContext.SaveChangesAsync();
        return listing;
    }

    protected async Task<Booking> SeedBookingAsync(
        Listing listing, User guest, DateOnly checkIn, DateOnly checkOut, BookingStatus status)
    {
        var booking = Booking.CreateBooking(guest.Id, checkIn, checkOut, listing, Guid.NewGuid());

        switch (status)
        {
            case BookingStatus.Confirmed:
                booking.Confirm();
                break;
            case BookingStatus.Rejected:
                booking.Reject();
                break;
            case BookingStatus.Cancelled:
                booking.Cancel(DateTimeOffset.UtcNow, listing.TimeZone!);
                break;
            case BookingStatus.Pending:
            case BookingStatus.Completed:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }

        DbContext.Bookings.Add(booking);
        await DbContext.SaveChangesAsync();
        return booking;
    }
}