using StayNGo.Domain.Enums;
using StayNGo.Domain.Interfaces;
using StayNGo.Domain.ValueObjects;

namespace StayNGo.Domain.Entities;

public class Booking : IEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public DateRange During { get; set; }
    public Money TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
    public Guid GuestUserId { get; set; }
    public User Guest { get; set; } = null!;
    public Listing Listing { get; set; } = null!;
    public Guid ListingId { get; set; }
}