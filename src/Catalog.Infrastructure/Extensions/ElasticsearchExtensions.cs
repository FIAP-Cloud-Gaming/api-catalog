namespace Catalog.Infrastructure.Extensions;

public static class ElasticsearchExtensions
{
    public static async Task EnsureElasticsearchIndexAsync(
        this ElasticsearchClient client,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        const string indexName = "games";

        try
        {
            logger.LogInformation("Testing Elasticsearch connection...");
            var pingResponse = await client.PingAsync(cancellationToken);

            if (!pingResponse.IsValidResponse)
            {
                var details = pingResponse.ApiCallDetails;
                logger.LogError(
                    "Failed to ping Elasticsearch. Uri: {Uri}, Status: {Status}, Method: {Method}, ResponseBody: {ResponseBody}",
                    details.Uri, details.HttpStatusCode, details.HttpMethod,
                    details.ResponseBodyInBytes != null ? System.Text.Encoding.UTF8.GetString(details.ResponseBodyInBytes) : "null");

                throw new InvalidOperationException($"Cannot connect to Elasticsearch at {details.Uri}. Status: {details.HttpStatusCode}");
            }

            logger.LogInformation("✅ Elasticsearch connection successful!");

            logger.LogInformation("Checking if index '{IndexName}' exists...", indexName);
            var existsResponse = await client.Indices.ExistsAsync(indexName, cancellationToken);

            if (existsResponse.Exists)
            {
                logger.LogInformation("Elasticsearch index '{IndexName}' already exists", indexName);
                return;
            }

            logger.LogInformation("Creating Elasticsearch index '{IndexName}'...", indexName);

        var createResponse = await client.Indices.CreateAsync(indexName, c => c
            .Mappings(m => m
                .Properties<GameSearchDocument>(p => p
                    // ID
                    .IntegerNumber(n => n.Id)

                    .Text(t => t.Title, td => td
                        .Fields(f => f
                            .Keyword("keyword", k => k.IgnoreAbove(256))))

                    .Text(t => t.Description)

                    .DoubleNumber(n => n.Price)
                    .Keyword(k => k.Currency)
                    .DoubleNumber(n => n.AverageRating)
                    .IntegerNumber(n => n.TotalReviews)

                    .Keyword(k => k.Genres)
                    .Keyword(k => k.Platforms)
                    .Keyword(k => k.Tags)
                    .Keyword(k => k.Status)

                    .Text(t => t.Publisher, td => td
                        .Fields(f => f
                            .Keyword("keyword", k => k.IgnoreAbove(256))))

                    .Text(t => t.Developer, td => td
                        .Fields(f => f
                            .Keyword("keyword", k => k.IgnoreAbove(256))))

                    .Keyword(k => k.CoverImageUrl)

                    .Date(d => d.ReleaseDate)
                    .Date(d => d.IndexedAt)
                ))
            .Settings(s => s
                .NumberOfShards(1)
                .NumberOfReplicas(1)
            ), cancellationToken);

            if (createResponse.IsValidResponse)
            {
                logger.LogInformation("Elasticsearch index '{IndexName}' created successfully", indexName);
            }
            else
            {
                var error = createResponse.ElasticsearchServerError?.Error?.Reason
                    ?? createResponse.DebugInformation
                    ?? "Unknown error";
                logger.LogError("Failed to create Elasticsearch index '{IndexName}': {Error}", indexName, error);
                throw new InvalidOperationException($"Failed to create Elasticsearch index '{indexName}': {error}");
            }
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            logger.LogError(ex, "Exception during Elasticsearch index creation");
            throw new InvalidOperationException($"Failed to create Elasticsearch index '{indexName}': {ex.Message}", ex);
        }
    }
}
