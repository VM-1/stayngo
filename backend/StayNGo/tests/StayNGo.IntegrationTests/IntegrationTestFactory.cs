using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StayNGo.Api.Services.Interfaces;
using StayNGo.Domain.Entities;
using StayNGo.Infrastructure.Persistence;
using StayNGo.IntegrationTests.FakeServices;
using Testcontainers.PostgreSql;

namespace StayNGo.IntegrationTests;

public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public User PrimaryUser { get; } = new()
    {
        Id = Guid.CreateVersion7(),
        ClerkId = "test-clerk-id",
        Email = "primary@integration.test",
        DisplayName = "Primary User",
    };
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<ICurrentUserService>();
            services.AddScoped<ICurrentUserService>(_ => new FakeCurrentUserService(PrimaryUser));
            ConfigureDbContext(services);
        });

        base.ConfigureWebHost(builder);
    }

    private void ConfigureDbContext(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<StayNGoDbContext>));

        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }

        services.AddDbContext<StayNGoDbContext>(options =>
        {
            options.UseNpgsql(_dbContainer.GetConnectionString()).UseSnakeCaseNamingConvention();
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        var options = new DbContextOptionsBuilder<StayNGoDbContext>()
            .UseNpgsql(_dbContainer.GetConnectionString())
            .UseSnakeCaseNamingConvention()
            .Options;

        await using (var migrationContext = new StayNGoDbContext(options))
        {
            await migrationContext.Database.MigrateAsync();
        }

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StayNGoDbContext>();

        dbContext.Users.Add(PrimaryUser);
        await dbContext.SaveChangesAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();

        await base.DisposeAsync();
    }
}