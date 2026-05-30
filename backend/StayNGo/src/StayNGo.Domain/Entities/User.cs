using StayNGo.Domain.Interfaces;

namespace StayNGo.Domain.Entities;

public class User : IEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string ClerkId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public bool IsAdmin { get; set; }
    
    public List<Listing> Listings { get; set; } = [];
    public List<Booking> Bookings { get; set; } = [];
}