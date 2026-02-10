namespace Catalog.API.Application.Models;

public sealed record GameViewModel
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required decimal PriceAmount { get; init; }
    public required string PriceCurrency { get; init; }
    public required GameStatus Status { get; init; }
    public DateTime? ReleaseDate { get; init; }
    public string? Publisher { get; init; }
    public string? Developer { get; init; }
    public string? CoverImageUrl { get; init; }
    public required IReadOnlyCollection<Genre> Genres { get; init; }
    public required IReadOnlyCollection<Platform> Platforms { get; init; }
    public decimal AverageRating { get; init; }
    public int TotalReviews { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }

    public static GameViewModel FromGame(Game game)
    {
        ArgumentNullException.ThrowIfNull(game);

        return new GameViewModel
        {
            Id = game.Id,
            Title = game.Title.Value,
            Description = game.Description,
            PriceAmount = game.Price.Amount,
            PriceCurrency = game.Price.Currency,
            Status = game.Status,
            ReleaseDate = game.ReleaseDate,
            Publisher = game.Publisher,
            Developer = game.Developer,
            CoverImageUrl = game.CoverImageUrl,
            Genres = [.. game.Genres],
            Platforms = [.. game.Platforms],
            AverageRating = game.AverageRating,
            TotalReviews = game.TotalReviews,
            CreatedAt = game.CreatedAt,
            UpdatedAt = game.UpdatedAt
        };
    }
}
