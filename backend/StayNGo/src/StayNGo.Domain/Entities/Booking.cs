using StayNGo.Domain.Enums;
using StayNGo.Domain.Exceptions;
using StayNGo.Domain.Interfaces;
using StayNGo.Domain.ValueObjects;

namespace StayNGo.Domain.Entities;

public class Booking : IEntity
{
    public required Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public DateOnly CheckIn { get; private set; }
    public DateOnly CheckOut { get; private set; }
    public Money TotalPrice { get; private set; } = null!;
    public BookingStatus Status { get; private set; }
    public Guid IdempotencyKey { get; private set; }
    public Guid GuestUserId { get; private set; }
    public User Guest { get; private set; } = null!;
    public Guid ListingId { get; private set; }
    public Listing Listing { get; private set; } = null!;

    private Booking() //EF
    {
    }

    public static Booking CreateBooking(Guid guestUserId, DateOnly checkIn, DateOnly checkOut, Listing listing,
        Guid idempotencyKey)
    {
        if (listing.Status != ListingStatus.Published)
        {
            throw new DomainException("Listing must be Published to book.");
        }

        if (checkIn >= checkOut)
        {
            throw new DomainException("Check-in must be before check-out.");
        }

        var nights = checkOut.DayNumber - checkIn.DayNumber;

        return new Booking
        {
            Id = Guid.CreateVersion7(),
            Status = BookingStatus.Pending,
            GuestUserId = guestUserId,
            ListingId = listing.Id,
            CheckIn = checkIn,
            CheckOut = checkOut,
            TotalPrice = listing.Price! * nights,
            IdempotencyKey = idempotencyKey
        };
    }

    public void Cancel()
    {
        if (Status != BookingStatus.Pending)
        {
            throw new DomainException("Booking cannot be cancelled.");
        }

        Status = BookingStatus.Cancelled;
    }

    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
        {
            throw new DomainException("Booking cannot be confirmed.");
        }

        Status = BookingStatus.Confirmed;
    }

    public void Reject()
    {
        if (Status != BookingStatus.Pending)
        {
            throw new DomainException("Booking cannot be rejected.");
        }

        Status = BookingStatus.Rejected;
    }

    public void Complete()
    {
        if (Status != BookingStatus.Confirmed)
        {
            throw new DomainException("Booking cannot be completed.");
        }

        Status = BookingStatus.Completed;
    }
}