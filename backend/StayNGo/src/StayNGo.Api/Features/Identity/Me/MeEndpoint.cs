using StayNGo.Api.Services.Interfaces;

namespace StayNGo.Api.Features.Identity.Me;

public class MeEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var groups = app.MapGroup("/identity/me").WithTags("Identity");

        groups.MapGet("", GetMe).RequireAuthorization();
    }

    private static async Task<IResult> GetMe(ICurrentUserService currentUserService)
    {
        var user = await currentUserService.GetOrProvisionAsync();

        return Results.Ok(new MeResponse(user.Id, user.Email, user.DisplayName));
    }
}