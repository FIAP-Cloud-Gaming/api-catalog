using Elastic.Clients.Elasticsearch.Aggregations;

namespace Catalog.Infrastructure.Repositories;

public class GameSearchRepository(ElasticsearchClient client)
    : IGameSearchRepository
{
    private readonly ElasticsearchClient _client = client
        ?? throw new ArgumentNullException(nameof(client));

    private const string IndexName = "games";
    private static readonly string[] sourceArray = ["title^3", "description", "tags^2"];

    public async Task IndexGameAsync(GameSearchDocument document, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(document);

        var response = await _client.IndexAsync(document, idx => idx.Index(IndexName).Id(document.Id), ct);

        if (!response.IsValidResponse)
            throw new InvalidOperationException($"Failed to index game {document.Id}: {response.ElasticsearchServerError?.Error.Reason}");
    }

    public async Task UpdateGameAsync(GameSearchDocument document, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(document);

        var response = await _client.UpdateAsync<GameSearchDocument, GameSearchDocument>(
            IndexName,
            document.Id.ToString(),
            u => u.Doc(document),
            ct);

        if (!response.IsValidResponse)
            throw new InvalidOperationException($"Failed to update game {document.Id}: {response.ElasticsearchServerError?.Error.Reason}");
    }

    public async Task DeleteGameAsync(int gameId, CancellationToken ct = default)
    {
        var response = await _client.DeleteAsync(IndexName, gameId, ct);

        if (!response.IsValidResponse && response.Result != Elastic.Clients.Elasticsearch.Result.NotFound)
            throw new InvalidOperationException($"Failed to delete game {gameId}: {response.ElasticsearchServerError?.Error.Reason}");
    }

    public async Task<(IEnumerable<GameSearchDocument> Items, long TotalCount)> SearchAsync(
        string searchTerm,
        List<Genre>? genres = null,
        List<Platform>? platforms = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        decimal? minRating = null,
        int pageIndex = 0,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var searchRequest = new SearchRequest(IndexName)
        {
            From = pageIndex * pageSize,
            Size = pageSize,
            TrackTotalHits = new Elastic.Clients.Elasticsearch.Core.Search.TrackHits(true)
        };

        var must = new List<Query>();
        var filter = new List<Query>();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            must.Add(new MultiMatchQuery
            {
                Query = searchTerm,
                Fields = sourceArray,
                Fuzziness = new Fuzziness("AUTO")
            });
        }

        if (genres is { Count: > 0 })
        {
            var genreStrings = genres.Select(g => g.ToString()).ToList();
            filter.Add(new TermsQuery
            {
                Field = "genres",
                Terms = new TermsQueryField(genreStrings.Select(FieldValue.String).ToArray())
            });
        }

        if (platforms is { Count: > 0 })
        {
            var platformStrings = platforms.Select(p => p.ToString()).ToList();
            filter.Add(new TermsQuery
            {
                Field = "platforms",
                Terms = new TermsQueryField(platformStrings.Select(FieldValue.String).ToArray())
            });
        }

        if (minPrice.HasValue || maxPrice.HasValue)
        {
            filter.Add(new NumberRangeQuery("price")
            {
                Gte = minPrice.HasValue ? (double)minPrice.Value : null,
                Lte = maxPrice.HasValue ? (double)maxPrice.Value : null
            });
        }

        if (minRating.HasValue)
        {
            filter.Add(new NumberRangeQuery("averageRating")
            {
                Gte = (double)minRating.Value
            });
        }

        if (must.Count == 0 && filter.Count == 0)
        {
            searchRequest.Query = new MatchAllQuery();
        }
        else
        {
            searchRequest.Query = new BoolQuery
            {
                Must = must.Count > 0 ? must : null,
                Filter = filter.Count > 0 ? filter : null
            };
        }

        searchRequest.Sort =
        [
            new SortOptions { Score = new ScoreSort { Order = SortOrder.Desc } },
            new SortOptions { Field = new FieldSort { Field = "averageRating", Order = SortOrder.Desc } }
        ];

        var response = await _client.SearchAsync<GameSearchDocument>(searchRequest, cancellationToken);

        if (!response.IsValidResponse)
            throw new InvalidOperationException($"Search failed: {response.ElasticsearchServerError?.Error.Reason}");

        return (response.Documents, response.Total);
    }

    public async Task<GameMetrics> GetGameMetricsAsync(CancellationToken ct)
    {
        var response = await _client.SearchAsync<GameSearchDocument>(s => s
            .Indices(IndexName)
            .Size(0) // Nao retorna documentos, apenas agregacoes
            .Aggregations(a => a
                .Add("top_genres", agg => agg.Terms(t => t.Field(f => f.Genres).Size(10)))
                .Add("top_platforms", agg => agg.Terms(t => t.Field(f => f.Platforms).Size(10)))
                .Add("avg_price", agg => agg.Avg(avg => avg.Field(f => f.Price)))
                .Add("price_ranges", agg => agg.Range(r => r
                    .Field(f => f.Price)
                    .Ranges(
                        new AggregationRange() { To = 20.0 },
                        new AggregationRange() { From = 20.0, To = 50.0 },
                        new AggregationRange() { From = 50.0, To = 100.0 },
                        new AggregationRange() { From = 100.0 }
                    )))
                .Add("most_reviewed", agg => agg.Terms(t => t
                    .Field(f => f.Title.Suffix("keyword"))
                    .Size(10)
                    .Order([new KeyValuePair<Field, SortOrder>("total_reviews", SortOrder.Desc)])
                ))
            ), ct);

        return MapToGameMetrics(response.Aggregations);
    }

    private static GameMetrics MapToGameMetrics(AggregateDictionary? aggregations)
    {
        if (aggregations is null)
            return new GameMetrics();

        var topGenres = new List<MetricBucket>();
        var genreAgg = aggregations.GetStringTerms("top_genres");
        if (genreAgg is not null)
        {
            topGenres.AddRange(genreAgg.Buckets.Select(b => new MetricBucket(b.Key.ToString(), b.DocCount)));
        }

        var topPlatforms = new List<MetricBucket>();
        var platformAgg = aggregations.GetStringTerms("top_platforms");
        if (platformAgg is not null)
        {
            topPlatforms.AddRange(platformAgg.Buckets.Select(b => new MetricBucket(b.Key.ToString(), b.DocCount)));
        }

        decimal avgPrice = 0;
        var avgAgg = aggregations.GetAverage("avg_price");
        if (avgAgg?.Value is not null)
        {
            avgPrice = (decimal)avgAgg.Value.Value;
        }

        var priceRanges = new List<PriceRangeBucket>();
        var rangeAgg = aggregations.GetRange("price_ranges");
        if (rangeAgg is not null)
        {
            priceRanges.AddRange(rangeAgg.Buckets.Select(b => new PriceRangeBucket(b.Key ?? string.Empty, b.From, b.To, b.DocCount)));
        }

        var mostReviewed = new List<MetricBucket>();
        var reviewedAgg = aggregations.GetStringTerms("most_reviewed");
        if (reviewedAgg is not null)
        {
            mostReviewed.AddRange(reviewedAgg.Buckets.Select(b => new MetricBucket(b.Key.ToString(), b.DocCount)));
        }

        return new GameMetrics
        {
            TopGenres = topGenres,
            TopPlatforms = topPlatforms,
            AveragePrice = avgPrice,
            PriceRanges = priceRanges,
            MostReviewed = mostReviewed
        };
    }

    public async Task<IReadOnlyList<GameSearchDocument>> GetRecommendationsAsync(
        IEnumerable<int> likedGameIds,
        int size = 10,
        CancellationToken ct = default)
    {
        var response = await _client.SearchAsync<GameSearchDocument>(s => s
            .Indices(IndexName)
            .Query(q => q
                .MoreLikeThis(mlt => mlt
                    .Fields(sourceArray)
                    .Like(likedGameIds.Select(id =>
                        new Like(new LikeDocument { Index = IndexName, Id = id.ToString() })).
                        ToList())
                    .MinTermFreq(1)
                    .MinDocFreq(1)
                    .MaxQueryTerms(12)
                )
            )
            .Size(size), ct);

        return [.. response.Documents];
    }
}
