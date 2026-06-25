using StayNGo.Api.Features.Common;
using StayNGo.Domain.Enums;

namespace StayNGo.Api.Features.Bookings.Reservations;

public record ReservationContract
{
    public required Guid Id { get; init; }
    public required DateOnly CheckIn { get; init; }
    public required DateOnly CheckOut { get; init; }
    public required MoneyContract TotalPrice { get; init; } = null!;
    public required BookingStatus Status { get; init; }
    public required UserShortContract Guest { get; init; } = null!;

    public static ReservationContract FromDomain(Domain.Entities.Booking b)
    {
        return new ReservationContract
        {
            Id = b.Id,
            CheckIn = b.CheckIn,
            CheckOut = b.CheckOut,
            TotalPrice = MoneyContract.From(b.TotalPrice)!,
            Status = b.Status,
            Guest = UserShortContract.FromDomain(b.Guest)
        };
    }

    public static List<ReservationContract> FromDomain(IEnumerable<Domain.Entities.Booking> bookings)
    {
        return bookings.Select(FromDomain).ToList();
    }
}