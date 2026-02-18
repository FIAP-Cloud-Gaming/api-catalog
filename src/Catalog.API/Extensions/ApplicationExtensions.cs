using EventBusRabbitMQ;

namespace Catalog.API.Extensions;

internal static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediator(assembly);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped(typeof(IPipelineBehavior<>), typeof(TransactionBehavior<>));
        services.AddScoped(typeof(IPipelineBehavior<>), typeof(LoggingBehavior<>));
        services.AddScoped(typeof(IPipelineBehavior<>), typeof(ValidationBehavior<>));

        services.AddScoped<ICatalogIntegrationEventService, CatalogIntegrationEventService>();

        return services;
    }

    public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CatalogDB")
            ?? throw new InvalidOperationException("CatalogDB connection string not found");

        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
            .AddNpgSql(
                connectionString,
                name: "catalog-db",
                tags: ["db", "postgres", "ready"]);

        return services;
    }

    public static IHostApplicationBuilder ConfigureEventBus(this IHostApplicationBuilder builder)
    {
        builder.AddRabbitMqEventBus(builder.Configuration.GetConnectionString("RabbitMQ")!, "catalog-service");

        return builder;
    }

    public static IHostApplicationBuilder ConfigureCatalogSerilog(this IHostApplicationBuilder builder)
    {
        var elasticsearchUri = builder.Configuration["Elasticsearch:Uri"]
            ?? throw new InvalidOperationException("Elasticsearch:Uri not found in configuration");

        builder.Services.AddSerilog(loggerConfiguration =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .Enrich.WithThreadId()
                .Enrich.WithProperty("Application", "Catalog.API")
                .WriteTo
                    .Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} <s:{SourceContext}>{NewLine}{Exception}")
                .WriteTo
                .Elasticsearch(
                    [new Uri(elasticsearchUri)],
                    opts =>
                    {
                        opts.DataStream = new Elastic.Ingest.Elasticsearch.DataStreams.DataStreamName("logs", "catalog-api");
                        opts.BootstrapMethod = Elastic.Ingest.Elasticsearch.BootstrapMethod.Failure;
                    },
                    transport =>
                    {
                        var username = builder.Configuration["Elasticsearch:Username"];
                        var password = builder.Configuration["Elasticsearch:Password"];

                        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                        {
                            transport.Authentication(new Elastic.Transport.BasicAuthentication(username, password));
                        }
                    });
        });

        return builder;
    }
}
