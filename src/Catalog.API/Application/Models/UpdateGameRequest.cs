namespace Catalog.API.Application.Models;

public sealed record UpdateGameRequest
{
    public string? Description { get; init; }
    public DateTime? ReleaseDate { get; init; }
    public string? Publisher { get; init; }
    public string? Developer { get; init; }
    public string? CoverImageUrl { get; init; }
    public IReadOnlyCollection<Genre>? Genres { get; init; }
    public IReadOnlyCollection<Platform>? Platforms { get; init; }
}
