using Microsoft.AspNetCore.Http.HttpResults;
using StayNGo.Api.Features.Listings;
using StayNGo.Api.Services.Interfaces;

namespace StayNGo.Api.Features.Bookings.UpdateReservation;

public class UpdateReservationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var groups = app.MapReservationGroup();

        groups.MapPost("{id:guid}/confirm", ConfirmReservation);
        groups.MapPost("{id:guid}/reject", RejectReservation);
    }

    private static async Task<Ok<BookingContract>> ConfirmReservation(IBookingService service, Guid id)
    {
        return TypedResults.Ok(await service.ConfirmReservation(id));
    }

    private static async Task<Ok<BookingContract>> RejectReservation(IBookingService service, Guid id)
    {
        return TypedResults.Ok(await service.RejectReservation(id));
    }
}
