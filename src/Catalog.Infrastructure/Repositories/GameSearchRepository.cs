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
}
