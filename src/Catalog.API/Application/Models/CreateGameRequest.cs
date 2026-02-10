namespace Catalog.API.Application.Models;

public sealed record CreateGameRequest
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required decimal PriceAmount { get; init; }
    public string PriceCurrency { get; init; } = "BRL";
    public required IReadOnlyCollection<Genre> Genres { get; init; }
    public required IReadOnlyCollection<Platform> Platforms { get; init; }
    public DateTime? ReleaseDate { get; init; }
    public string? Publisher { get; init; }
    public string? Developer { get; init; }
    public string? CoverImageUrl { get; init; }
}
