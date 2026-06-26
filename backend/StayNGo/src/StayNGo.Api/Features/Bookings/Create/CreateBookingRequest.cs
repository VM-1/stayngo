namespace StayNGo.Api.Features.Bookings.Create;

public record CreateBookingRequest(Guid ListingId, DateOnly CheckIn, DateOnly CheckOut, Guid IdempotencyKey);