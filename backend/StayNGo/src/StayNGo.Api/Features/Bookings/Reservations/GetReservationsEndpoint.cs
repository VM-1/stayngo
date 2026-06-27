using Microsoft.AspNetCore.Http.HttpResults;
using StayNGo.Api.Features.Common;
using StayNGo.Api.Features.Listings;
using StayNGo.Api.Services.Interfaces;

namespace StayNGo.Api.Features.Bookings.Reservations;

public class GetReservationsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var groups = app.MapReservationGroup();

        groups.MapGet("", GetReservations);
    }

    private static async Task<Ok<PageResult<ReservationContract>>> GetReservations(
        IBookingService service, [AsParameters] GetBookingFilter filter)
    {
        return TypedResults.Ok(await service.GetReservations(filter));
    }
}
