namespace Catalog.Domain.CacheModels;

public class GameCacheModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "BRL";
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public string? CoverImageUrl { get; set; }
    public DateTime CachedAt { get; set; }

    public static GameCacheModel FromGame(Game game)
    {
        return new GameCacheModel
        {
            Id = game.Id,
            Title = game.Title.Value,
            Price = game.Price.Amount,
            Currency = game.Price.Currency,
            AverageRating = game.AverageRating,
            TotalReviews = game.TotalReviews,
            CoverImageUrl = game.CoverImageUrl,
            CachedAt = DateTime.UtcNow
        };
    }

    public string GetCacheKey() => $"catalog:game:{Id}";
}
