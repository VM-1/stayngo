using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StayNGo.Api.Features;
using StayNGo.Api.Services.Interfaces;

namespace StayNGo.Api.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpoints(typeof(Program).Assembly);
        services.AddEndpointsApiExplorer();

        ConfigureTokenValidation(services, configuration);

        services.AddApplication(configuration);

        return services;
    }

    private static void ConfigureTokenValidation(IServiceCollection services, IConfiguration configuration)
    {
        var authority = configuration["Clerk:Authority"];

        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,

                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });
    }

    private static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}