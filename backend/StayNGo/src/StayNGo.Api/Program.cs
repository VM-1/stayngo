using System.Reflection;
using Serilog;
using StayNGo.Api.Features;
using StayNGo.Api.Services;
using StayNGo.Infrastructure.Services;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration).WriteTo.Console());

builder.Services.AddApi(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));


app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
    
app.MapEndpoints();



app.Run();