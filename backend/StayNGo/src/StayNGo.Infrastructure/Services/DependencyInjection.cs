using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StayNGo.Infrastructure.Persistence;

namespace StayNGo.Infrastructure.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddPersistence(configuration);
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("StayNGo");

        services.AddDbContext<StayNGoDbContext>(opt => opt
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention()
        );
        
        return services;
    }
}