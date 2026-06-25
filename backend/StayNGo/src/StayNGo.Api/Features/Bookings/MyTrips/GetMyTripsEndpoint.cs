using StayNGo.Api.Features.Listings;
using StayNGo.Api.Services.Interfaces;

namespace StayNGo.Api.Features.Bookings.MyTrips;

public class GetMyTripsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var groups = app.MapMyTripsGroup();

        groups.MapGet("", GetMyTrips);
    }

    private static async Task<IResult> GetMyTrips(IBookingService service, [AsParameters] GetBookingFilter filter)
    {
        return Results.Ok(await service.GetMyTrips(filter));
    }
}