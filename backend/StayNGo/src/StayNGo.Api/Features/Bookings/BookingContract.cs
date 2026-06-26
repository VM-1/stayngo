using StayNGo.Api.Features.Common;
using StayNGo.Domain.Enums;

namespace StayNGo.Api.Features.Bookings;

public record BookingContract
{
    public Guid Id { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public MoneyContract TotalPrice { get; set; } = null!;
    public BookingStatus Status { get; set; }

    public static BookingContract FromDomain(Domain.Entities.Booking b)
    {
        return new BookingContract
        {
            Id = b.Id,
            CheckIn = b.CheckIn,
            CheckOut = b.CheckOut,
            TotalPrice = MoneyContract.From(b.TotalPrice)!,
            Status = b.Status
        };
    }

    public static List<BookingContract> FromDomain(IEnumerable<Domain.Entities.Booking> bookings)
    {
        return bookings.Select(FromDomain).ToList();
    }
}