using StayNGo.Domain.Enums;
using StayNGo.Domain.Exceptions;
using StayNGo.Domain.Interfaces;
using StayNGo.Domain.ValueObjects;

namespace StayNGo.Domain.Entities;

public class Listing : IEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public List<string> ImageUrls { get; private set; } = [];
    public string? MainImageUrl { get; private set; }
    public string? Location { get; private set; }
    public IanaTimeZone? TimeZone { get; private set; }
    public ListingStatus Status { get; private set; }
    public Money? Price { get; private set; }
    public int? Capacity { get; private set; }
    public Guid OwnerUserId { get; private set; }
    public User Owner { get; private set; } = null!;

    public List<Booking> Bookings { get; private set; } = [];

    private Listing()
    {
    }

    public static Listing StartDraft(Guid ownerUserId)
    {
        return new Listing
        {
            Id = Guid.CreateVersion7(),
            OwnerUserId = ownerUserId,
            Status = ListingStatus.Draft
        };
    }

    public void UpdateDraftDetails(string? title, string? description,
        string? location, string? timeZone,
        string? mainImageUrl, List<string> imageUrls,
        Money? price, int? capacity)
    {
        if (Status != ListingStatus.Draft)
        {
            throw new DomainException("Only a draft listing can be edited this way.");
        }

        Title = title;
        Description = description;
        MainImageUrl = mainImageUrl;
        Location = location;
        TimeZone = IanaTimeZone.From(timeZone);
        ImageUrls = imageUrls;
        Price = price;
        Capacity = capacity;
    }

    public void UpdatePublishedDetails(string description, string mainImageUrl, List<string> imageUrls, Money price)
    {
        if (Status != ListingStatus.Published)
        {
            throw new DomainException("Only a published listing can be edited this way.");
        }

        Description = description;
        MainImageUrl = mainImageUrl;
        ImageUrls = imageUrls;
        Price = price;
    }

    public void Publish()
    {
        if (Status != ListingStatus.Draft)
        {
            throw new DomainException("Only a draft listing can be published.");
        }

        var missing = new List<string>();

        if (string.IsNullOrWhiteSpace(Title)) missing.Add(nameof(Title));
        if (string.IsNullOrWhiteSpace(Location)) missing.Add(nameof(Location));
        if (string.IsNullOrWhiteSpace(Description)) missing.Add(nameof(Description));
        if (string.IsNullOrWhiteSpace(MainImageUrl)) missing.Add(nameof(MainImageUrl));

        if (Price is null) missing.Add(nameof(Price));
        if (Capacity is null) missing.Add(nameof(Capacity));
        if (TimeZone is null) missing.Add(nameof(TimeZone));
        if (ImageUrls.Count == 0) missing.Add(nameof(ImageUrls));

        if (missing.Count > 0)
        {
            throw new DomainException($"Missing: {string.Join(", ", missing)}");
        }

        Status = ListingStatus.Published;
    }

    public void Archive()
    {
        if (Status == ListingStatus.Archived)
        {
            throw new DomainException("Listing is already archived.");
        }

        Status = ListingStatus.Archived;
    }
}