namespace StayNGo.Api.Features.Listings;

internal static class ListingRoutes
{

    public static RouteGroupBuilder MapHostListingsGroup(this IEndpointRouteBuilder endpoints) => endpoints
        .MapGroup("/host/listings")
        .WithTags("Listings (host)").RequireAuthorization();

    public static RouteGroupBuilder MapListingGroup(this IEndpointRouteBuilder endpoints) => endpoints
        .MapGroup("/listings")
        .WithTags("Listings (browse)");
}