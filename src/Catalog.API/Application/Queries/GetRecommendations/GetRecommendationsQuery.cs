namespace Catalog.API.Application.Queries.GetRecommendations;

public record GetRecommendationsQuery(
    IReadOnlyCollection<int> GameIds,
    int Size = 10) : IQuery<IReadOnlyList<GameViewModel>>;

public class GetRecommendationsQueryHandler(IGameSearchRepository gameSearchRepository)
    : IQueryHandler<GetRecommendationsQuery, IReadOnlyList<GameViewModel>>
{
    public async Task<IReadOnlyList<GameViewModel>> Handle(
        GetRecommendationsQuery query,
        CancellationToken cancellationToken)
    {
        var recommendations = await gameSearchRepository.GetRecommendationsAsync(
            query.GameIds,
            query.Size,
            cancellationToken);

        return recommendations.Select(doc => new GameViewModel
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
    }
}
