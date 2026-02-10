namespace Catalog.API.Application.Models;

public sealed record SearchGamesRequest
{
    public string? SearchTerm { get; init; }
    public string? Genres { get; init; }
    public string? Platforms { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public GameStatus? Status { get; init; }
    public decimal? MinRating { get; init; }
    public string? Publisher { get; init; }
    public string? Developer { get; init; }
    public DateTime? ReleaseDateFrom { get; init; }
    public DateTime? ReleaseDateTo { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string SortBy { get; init; } = "CreatedAt";
    public bool SortDescending { get; init; } = true;
}
