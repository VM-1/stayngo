using StayNGo.Api.Features.Listings;
using StayNGo.Api.Services.Interfaces;
using StayNGo.Domain.Enums;

namespace StayNGo.Api.Features.Bookings.UpdateReservation;

public class UpdateReservationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var groups = app.MapReservationGroup();

        groups.MapPost("{id:guid}/confirm", ConfirmReservation);
        groups.MapPost("{id:guid}/reject", RejectReservation);
    }

    private static async Task<IResult> ConfirmReservation(IBookingService service, Guid id)
    {
        return Results.Ok(await service.ConfirmReservation(id));
    }

    private static async Task<IResult> RejectReservation(IBookingService service, Guid id)
    {
        return Results.Ok(await service.RejectReservation(id));
    }
}