using Microsoft.AspNetCore.Http.HttpResults;
using StayNGo.Api.Features.Common;
using StayNGo.Api.Features.Listings;
using StayNGo.Api.Services.Interfaces;

namespace StayNGo.Api.Features.Bookings.MyTrips;

public class GetMyTripsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var groups = app.MapMyTripsGroup();

        groups.MapGet("", GetMyTrips);
        groups.MapPut("{id:guid}/cancel", CancelTrip);
    }

    private static async Task<Ok<PageResult<BookingContract>>> GetMyTrips(
        IBookingService service, [AsParameters] GetBookingFilter filter)
    {
        return TypedResults.Ok(await service.GetMyTrips(filter));
    }

    private static async Task<Ok<BookingContract>> CancelTrip(IBookingService service, Guid id)
    {
        return TypedResults.Ok(await service.CancelTrip(id));
    }
}
