namespace StayNGo.Api.Features.Listings;

internal static class ListingRoutes
{
    public static RouteGroupBuilder MapHostListingsGroup(this IEndpointRouteBuilder endpoints) => endpoints
        .MapGroup("/host/listings")
        .WithTags("Listings (host)")
        .RequireAuthorization();

    public static RouteGroupBuilder MapListingGroup(this IEndpointRouteBuilder endpoints) => endpoints
        .MapGroup("/listings")
        .WithTags("Listings (browse)");

    public static RouteGroupBuilder MapBookingGroup(this IEndpointRouteBuilder endpoints) => endpoints
        .MapGroup("/bookings")
        .WithTags("Bookings")
        .RequireAuthorization();

    public static RouteGroupBuilder MapReservationGroup(this IEndpointRouteBuilder endpoints) => endpoints
        .MapGroup("/reservations")
        .WithTags("Reservations")
        .RequireAuthorization();
    public static RouteGroupBuilder MapMyTripsGroup(this IEndpointRouteBuilder endpoints) => endpoints
        .MapGroup("/me/trips")
        .WithTags("My trips")
        .RequireAuthorization();
}