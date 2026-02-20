using Catalog.API.Apis;
using Catalog.API.Extensions;
using Catalog.API.HostedServices;
using Catalog.API.Middleware;
using Core.Defaults;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureCatalogSerilog();
builder.ConfigureOpenTelemetry();
builder.ConfigureEventBus();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApplication();

builder.Services.AddCustomHealthChecks(builder.Configuration);

builder.Services.AddOpenApiDocumentation();

builder.Services.AddHostedService<DatabaseInitializerService>();

var app = builder.Build();

app.UseExceptionHandling();

app.ApplyMigrations();

app.MapDefaultEndpoints();

app.UseOpenApiDocumentation();

app.UseAuthentication();
app.UseAuthorization();

app.MapGamesApi();

app.Run();
