namespace Catalog.API.HostedServices;

public class DatabaseInitializerService(
    IServiceProvider serviceProvider,
    ILogger<DatabaseInitializerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const int maxRetries = 15;
        const int baseDelaySeconds = 3;

        logger.LogInformation("Starting database initialization...");

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();

                logger.LogInformation("Initializing databases (attempt {Attempt}/{Max})...", attempt, maxRetries);

                var elasticsearchClient = scope.ServiceProvider.GetRequiredService<ElasticsearchClient>();
                await elasticsearchClient.EnsureElasticsearchIndexAsync(logger, stoppingToken);

                var mongoDatabase = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
                await mongoDatabase.EnsureMongoDbIndexesAsync(logger, stoppingToken);

                logger.LogInformation("Database initialization completed successfully");
                return;
            }
            catch (Exception ex) when (attempt < maxRetries && !stoppingToken.IsCancellationRequested)
            {
                var delay = TimeSpan.FromSeconds(baseDelaySeconds * attempt);
                logger.LogWarning(ex, "Database initialization failed. Retrying in {Delay}s (attempt {Attempt}/{Max})...",
                    delay.TotalSeconds, attempt, maxRetries);
                await Task.Delay(delay, stoppingToken);
            }
        }

        try
        {
            using var scope = serviceProvider.CreateScope();

            var elasticsearchClient = scope.ServiceProvider.GetRequiredService<ElasticsearchClient>();
            await elasticsearchClient.EnsureElasticsearchIndexAsync(logger, stoppingToken);

            var mongoDatabase = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
            await mongoDatabase.EnsureMongoDbIndexesAsync(logger, stoppingToken);

            logger.LogInformation("Database initialization completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database initialization failed after {MaxRetries} attempts. The application will continue but some features may not work.", maxRetries);
        }
    }
}
