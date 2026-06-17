using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using StayNGo.Api.Features;
using StayNGo.Api.Services;
using StayNGo.Infrastructure.Persistence;
using StayNGo.Infrastructure.Services;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration).WriteTo.Console());

builder.Services.AddApi(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

await ValidatePendingMigrations(app);

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.UseExceptionHandler();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();       
    app.MapScalarApiReference(options =>
    {
        options
            .AddPreferredSecuritySchemes("Bearer")       
            .EnablePersistentAuthentication();     
    });
}


app.Run();

async Task ValidatePendingMigrations(WebApplication webApplication)
{
    using var scope = webApplication.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<StayNGoDbContext>();
    var pending = (await db.Database.GetPendingMigrationsAsync()).ToList();
    if (pending.Count > 0)
    {
        throw new InvalidOperationException(
            $"Database has {pending.Count} pending migration(s): {string.Join(", ", pending)}. " +
            "Run 'dotnet ef database update' (ASPNETCORE_ENVIRONMENT=Development) before starting.");
    }
}