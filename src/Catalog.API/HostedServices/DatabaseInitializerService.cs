namespace Catalog.API.HostedServices;

public class DatabaseInitializerService(
    IServiceProvider serviceProvider,
    ILogger<DatabaseInitializerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting database initialization...");

        using var scope = serviceProvider.CreateScope();

        // Elasticsearch - will throw and stop app if it fails
        var elasticsearchClient = scope.ServiceProvider.GetRequiredService<ElasticsearchClient>();
        await elasticsearchClient.EnsureElasticsearchIndexAsync(logger, stoppingToken);

        // MongoDB
        var mongoDatabase = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
        await mongoDatabase.EnsureMongoDbIndexesAsync(logger, stoppingToken);

        logger.LogInformation("✅ Database initialization completed successfully");
    }
}
