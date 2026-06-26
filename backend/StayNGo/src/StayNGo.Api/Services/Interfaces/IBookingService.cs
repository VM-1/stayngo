using StayNGo.Api.Features.Bookings;
using StayNGo.Api.Features.Bookings.Create;
using StayNGo.Api.Features.Bookings.Reservations;
using StayNGo.Api.Features.Common;

namespace StayNGo.Api.Services.Interfaces;

public interface IBookingService
{
    Task<BookingContract> CreateAsync(CreateBookingRequest request, Guid idempotencyKey);
    Task<PageResult<BookingContract>> GetMyTrips(GetBookingFilter filter);
    Task<PageResult<ReservationContract>> GetReservations(GetBookingFilter filter);
    Task<BookingContract> ConfirmReservation(Guid bookingId);
    Task<BookingContract> RejectReservation(Guid bookingId);
}