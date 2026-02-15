namespace Catalog.API.Application.Queries.SearchGames;

public record SearchGamesQuery(
    string? SearchTerm,
    IReadOnlyCollection<Genre>? Genres,
    IReadOnlyCollection<Platform>? Platforms,
    decimal? MinPrice,
    decimal? MaxPrice,
    decimal? MinRating,
    int PageNumber,
    int PageSize) : IQuery<PagedResponse<GameViewModel>>
{
    public static SearchGamesQuery FromRequest(SearchGamesRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        IReadOnlyCollection<Genre>? genres = null;
        if (!string.IsNullOrWhiteSpace(request.Genres))
        {
            genres = [.. request.Genres
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(g => Enum.Parse<Genre>(g, ignoreCase: true))];
        }

        IReadOnlyCollection<Platform>? platforms = null;
        if (!string.IsNullOrWhiteSpace(request.Platforms))
        {
            platforms = [.. request.Platforms
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(p => Enum.Parse<Platform>(p, ignoreCase: true))];
        }

        return new SearchGamesQuery(
            request.SearchTerm,
            genres,
            platforms,
            request.MinPrice,
            request.MaxPrice,
            request.MinRating,
            request.PageNumber,
            request.PageSize);
    }
}

public class SearchGamesQueryHandler(IGameSearchRepository gameSearchRepository) : IQueryHandler<SearchGamesQuery, PagedResponse<GameViewModel>>
{
    public async Task<PagedResponse<GameViewModel>> Handle(SearchGamesQuery query, CancellationToken cancellationToken)
    {
        var (searchResults, totalCount) = await gameSearchRepository.SearchAsync(
            query.SearchTerm ?? string.Empty,
            query.Genres?.ToList(),
            query.Platforms?.ToList(),
            query.MinPrice,
            query.MaxPrice,
            query.MinRating,
            query.PageNumber - 1,
            query.PageSize,
            cancellationToken);

        var gameViewModels = searchResults.Select(doc => new GameViewModel
        {
            Id = doc.Id,
            Title = doc.Title,
            Description = doc.Description,
            PriceAmount = doc.Price,
            PriceCurrency = doc.Currency,
            Status = Enum.Parse<GameStatus>(doc.Status),
            ReleaseDate = doc.ReleaseDate,
            Publisher = doc.Publisher,
            Developer = doc.Developer,
            CoverImageUrl = doc.CoverImageUrl,
            Genres = [.. doc.Genres.Select(g => Enum.Parse<Genre>(g))],
            Platforms = [.. doc.Platforms.Select(p => Enum.Parse<Platform>(p))],
            AverageRating = doc.AverageRating,
            TotalReviews = doc.TotalReviews,
            CreatedAt = doc.IndexedAt,
            UpdatedAt = null
        }).ToList();

        return new PagedResponse<GameViewModel>(
            gameViewModels,
            query.PageNumber,
            query.PageSize,
            (int)totalCount);
    }
}
