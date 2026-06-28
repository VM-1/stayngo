using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using StayNGo.Api.Features.Common;
using StayNGo.Api.Features.Listings;
using StayNGo.Api.Services.Interfaces;

namespace StayNGo.Api.Features.Bookings.Create;

public class CreateBookingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var groups = app.MapBookingGroup();

        groups.MapPost("", CreateBooking).WithValidation<CreateBookingRequest>();
    }

    private static async Task<Results<Ok<BookingContract>, BadRequest<string>>> CreateBooking(
        IBookingService service,
        CreateBookingRequest request,
        [FromHeader(Name = "Idempotency-Key")] Guid? idempotencyKey)
    {
        if (!idempotencyKey.HasValue || idempotencyKey == Guid.Empty)
        {
            return TypedResults.BadRequest("Idempotency-Key header is required.");
        }

        return TypedResults.Ok(await service.CreateAsync(request, idempotencyKey.Value));
    }
}
