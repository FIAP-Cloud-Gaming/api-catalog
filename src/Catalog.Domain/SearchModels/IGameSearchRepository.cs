namespace Catalog.Domain.SearchModels;

public interface IGameSearchRepository
{
    Task IndexGameAsync(GameSearchDocument document, CancellationToken cancellationToken = default);
    Task UpdateGameAsync(GameSearchDocument document, CancellationToken cancellationToken = default);
    Task DeleteGameAsync(int gameId, CancellationToken cancellationToken = default);

    Task<(IEnumerable<GameSearchDocument> Items, long TotalCount)> SearchAsync(
        string searchTerm,
        List<Genre>? genres = null,
        List<Platform>? platforms = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        decimal? minRating = null,
        int pageIndex = 0,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<GameMetrics> GetGameMetricsAsync(CancellationToken ct = default);

    Task<IReadOnlyList<GameSearchDocument>> GetRecommendationsAsync(
        IEnumerable<int> likedGameIds,
        int size = 10,
        CancellationToken ct = default);
}
