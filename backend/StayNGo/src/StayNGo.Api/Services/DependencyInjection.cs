using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StayNGo.Api.Exceptions;
using StayNGo.Api.Features;
using StayNGo.Api.Services.Interfaces;
using StayNGo.Api.Services.OpenApi;

namespace StayNGo.Api.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

            options.AddDefaultPolicy(policy =>
                policy.WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });

        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });
        
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        
        services.AddEndpoints(typeof(Program).Assembly);
        services.AddEndpointsApiExplorer();
        services.AddValidatorsFromAssembly(typeof(Program).Assembly);

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
        services.AddScoped<IListingService, ListingService>();
        services.AddScoped<IBookingService, BookingService>();

        return services;
    }
}