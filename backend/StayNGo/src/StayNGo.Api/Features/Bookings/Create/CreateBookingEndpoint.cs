using Microsoft.AspNetCore.Mvc;
using StayNGo.Api.Features.Listings;
using StayNGo.Api.Services.Interfaces;

namespace StayNGo.Api.Features.Bookings.Create;

public class CreateBookingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var groups = app.MapBookingGroup();

        groups.MapPost("", CreateBooking);
    }

    private static async Task<IResult> CreateBooking(IBookingService service, CreateBookingRequest request,
        [FromHeader(Name = "Idempotency-Key")] Guid? idempotencyKey)
    {
        if (!idempotencyKey.HasValue || idempotencyKey == Guid.Empty)
        {
            return Results.BadRequest("Idempotency-Key header is required.");
        }

        return Results.Ok(await service.CreateAsync(request, idempotencyKey.Value));
    }
}