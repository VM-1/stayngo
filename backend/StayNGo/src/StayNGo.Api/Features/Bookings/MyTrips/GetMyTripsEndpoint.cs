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

    private static async Task<IResult> GetMyTrips(IBookingService service, [AsParameters] GetBookingFilter filter)
    {
        return Results.Ok(await service.GetMyTrips(filter));
    }
    private static async Task<IResult> CancelTrip(IBookingService service, Guid id)
    {
        return Results.Ok(await service.CancelTrip(id));
    }
    
}