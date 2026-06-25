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

    private static async Task<IResult> GetReservations(IBookingService service, [AsParameters] GetBookingFilter filter)
    {
        return Results.Ok(await service.GetReservations(filter));
    }
}