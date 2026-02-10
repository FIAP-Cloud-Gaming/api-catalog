namespace Catalog.Infrastructure.Extensions;

public static class MongoDbExtensions
{
    public static async Task EnsureMongoDbIndexesAsync(
        this IMongoDatabase database,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating MongoDB indexes...");

        var reviewsCollection = database.GetCollection<Review>("reviews");

        await reviewsCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<Review>(
                Builders<Review>.IndexKeys.Ascending(r => r.GameId),
                new CreateIndexOptions { Name = "idx_reviews_gameid", Background = true }),
            cancellationToken: cancellationToken);

        await reviewsCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<Review>(
                Builders<Review>.IndexKeys.Ascending(r => r.UserId),
                new CreateIndexOptions { Name = "idx_reviews_userid", Background = true }),
            cancellationToken: cancellationToken);

        await reviewsCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<Review>(
                Builders<Review>.IndexKeys
                    .Ascending(r => r.GameId)
                    .Ascending(r => r.UserId),
                new CreateIndexOptions { Name = "idx_reviews_game_user", Unique = true, Background = true }),
            cancellationToken: cancellationToken);

        await reviewsCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<Review>(
                Builders<Review>.IndexKeys.Descending(r => r.CreatedAt),
                new CreateIndexOptions { Name = "idx_reviews_createdat", Background = true }),
            cancellationToken: cancellationToken);

        logger.LogInformation("Created indexes for 'reviews' collection");

        var priceHistoryCollection = database.GetCollection<GamePriceHistory>("price_histories");

        await priceHistoryCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<GamePriceHistory>(
                Builders<GamePriceHistory>.IndexKeys.Ascending(h => h.GameId),
                new CreateIndexOptions { Name = "idx_pricehistory_gameid", Unique = true, Background = true }),
            cancellationToken: cancellationToken);

        logger.LogInformation("Created indexes for 'price_histories' collection");
        logger.LogInformation("MongoDB indexes created successfully");
    }
}
