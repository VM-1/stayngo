namespace StayNGo.Api.Features.Listings;

internal static class ListingRoutes
{
    public const string Base = "/host/listings";

    public static RouteGroupBuilder MapListingsGroup(this IEndpointRouteBuilder endpoints) => endpoints.MapGroup(Base)
        .WithTags("Listings (host)").RequireAuthorization();
}