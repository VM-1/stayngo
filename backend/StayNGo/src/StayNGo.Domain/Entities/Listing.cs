using StayNGo.Domain.Enums;
using StayNGo.Domain.Interfaces;
using StayNGo.Domain.ValueObjects;

namespace StayNGo.Domain.Entities;

public class Listing : IEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<string> ImageUrls { get; set; } = [];
    public string MainImageUrl { get; set; } = null!;
    public string Location { get; set; } = null!;
    public string TimeZoneId { get; set; } = null!;
    public ListingStatus Status { get; set; }
    public Money Price { get; set; }
    public int Capacity { get; set; }
    public Guid OwnerUserId { get; set; }
    public User Owner { get; set; } = null!;

    public List<Booking>? Bookings { get; set; }
}