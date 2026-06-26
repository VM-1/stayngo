using StayNGo.Api.Features.Common;
using StayNGo.Domain.Enums;

namespace StayNGo.Api.Features.Bookings;

public class GetBookingFilter : PaginationFilter
{
    public BookingStatus? Status { get; set; }
}