namespace Catalog.Domain.SearchModels;

public class GameSearchDocument
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "BRL";
    public List<string> Genres { get; set; } = [];
    public List<string> Platforms { get; set; } = [];
    public string Status { get; set; } = string.Empty;
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? Publisher { get; set; }
    public string? Developer { get; set; }
    public string? CoverImageUrl { get; set; }
    public List<string> Tags { get; set; } = [];
    public DateTime IndexedAt { get; set; }

    public static GameSearchDocument FromGame(Game game)
    {
        return new GameSearchDocument
        {
            Id = game.Id,
            Title = game.Title.Value,
            Description = game.Description,
            Price = game.Price.Amount,
            Currency = game.Price.Currency,
            Genres = [.. game.Genres.Select(g => g.ToString())],
            Platforms = [.. game.Platforms.Select(p => p.ToString())],
            Status = game.Status.ToString(),
            AverageRating = game.AverageRating,
            TotalReviews = game.TotalReviews,
            ReleaseDate = game.ReleaseDate,
            Publisher = game.Publisher,
            Developer = game.Developer,
            CoverImageUrl = game.CoverImageUrl,
            Tags = GenerateTags(game),
            IndexedAt = DateTime.UtcNow
        };
    }

    private static List<string> GenerateTags(Game game)
    {
        var tags = new List<string>();

        tags.AddRange(game.Title.Value.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries));

        tags.AddRange(game.Genres.Select(g => g.ToString().ToLower()));

        tags.AddRange(game.Platforms.Select(p => p.ToString().ToLower()));

        return [.. tags.Distinct()];
    }
}
