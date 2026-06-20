using Microsoft.Extensions.DependencyInjection;
using StayNGo.Infrastructure.Persistence;

namespace StayNGo.IntegrationTests;

public class BaseIntegrationTests : IClassFixture<IntegrationTestFactory>
{
    protected readonly IServiceScope Scope;
    protected readonly StayNGoDbContext DbContext;
    protected IntegrationTestFactory Factory { get; set; }

    protected BaseIntegrationTests(IntegrationTestFactory factory)
    {
        Factory = factory;
        Scope = factory.Services.CreateScope();
        
        DbContext = Scope.ServiceProvider.GetRequiredService<StayNGoDbContext>();
    }

}