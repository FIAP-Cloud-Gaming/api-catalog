namespace Catalog.Domain.AggregatesModel.GameAggregate;

public class Game : EntityWithNotification, IAggregateRoot
{
    public GameTitle Title { get; private set; }
    public string Description { get; private set; }
    public Price Price { get; private set; }
    public GameStatus Status { get; private set; }
    public DateTime? ReleaseDate { get; private set; }
    public string? Publisher { get; private set; }
    public string? Developer { get; private set; }
    public string? CoverImageUrl { get; private set; }

    private readonly List<Genre> _genres = [];
    public IReadOnlyCollection<Genre> Genres => _genres.AsReadOnly();

    private readonly List<Platform> _platforms = [];
    public IReadOnlyCollection<Platform> Platforms => _platforms.AsReadOnly();

    public decimal AverageRating { get; private set; }
    public int TotalReviews { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    protected Game()
    {
        Title = null!;
        Description = string.Empty;
        Price = null!;
    }

    private Game(
        GameTitle title,
        string description,
        Price price,
        IEnumerable<Genre> genres,
        IEnumerable<Platform> platforms,
        DateTime? releaseDate,
        string? publisher,
        string? developer,
        string? coverImageUrl)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? string.Empty;
        Price = price ?? throw new ArgumentNullException(nameof(price));
        Status = GameStatus.Draft;
        ReleaseDate = releaseDate;
        Publisher = publisher;
        Developer = developer;
        CoverImageUrl = coverImageUrl;
        CreatedAt = DateTime.UtcNow;

        if (genres == null || !genres.Any())
            throw new CatalogDomainException("Game must have at least one genre.");

        if (platforms == null || !platforms.Any())
            throw new CatalogDomainException("Game must have at least one platform.");

        _genres.AddRange(genres.Distinct());
        _platforms.AddRange(platforms.Distinct());

        AddNotification(new GameCreatedDomainEvent(Id, Title.Value));
    }

    public static Game Create(
        GameTitle title,
        string description,
        Price price,
        IEnumerable<Genre> genres,
        IEnumerable<Platform> platforms,
        DateTime? releaseDate = null,
        string? publisher = null,
        string? developer = null,
        string? coverImageUrl = null)
    {
        return new Game(title, description, price, genres, platforms,
            releaseDate, publisher, developer, coverImageUrl);
    }

    public void ChangePrice(Price newPrice)
    {
        ArgumentNullException.ThrowIfNull(newPrice);

        var oldPrice = Price;
        Price = newPrice;
        UpdatedAt = DateTime.UtcNow;

        AddNotification(new GamePriceChangedDomainEvent(Id, oldPrice, newPrice));
    }

    public void UpdateDetails(
        string? description = null,
        DateTime? releaseDate = null,
        string? publisher = null,
        string? developer = null,
        string? coverImageUrl = null)
    {
        if (description != null)
            Description = description;

        if (releaseDate.HasValue)
            ReleaseDate = releaseDate;

        if (publisher != null)
            Publisher = publisher;

        if (developer != null)
            Developer = developer;

        if (coverImageUrl != null)
            CoverImageUrl = coverImageUrl;

        UpdatedAt = DateTime.UtcNow;

        AddNotification(new GameUpdatedDomainEvent(Id, Title.Value));
    }

    public void AddGenre(Genre genre)
    {
        if (!_genres.Contains(genre))
        {
            _genres.Add(genre);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemoveGenre(Genre genre)
    {
        if (_genres.Count <= 1)
            throw new CatalogDomainException("Game must have at least one genre.");

        _genres.Remove(genre);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddPlatform(Platform platform)
    {
        if (!_platforms.Contains(platform))
        {
            _platforms.Add(platform);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemovePlatform(Platform platform)
    {
        if (_platforms.Count <= 1)
            throw new CatalogDomainException("Game must have at least one platform.");

        _platforms.Remove(platform);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Publish()
    {
        if (Status == GameStatus.Published)
            throw new CatalogDomainException("Game is already published.");

        Status = GameStatus.Published;
        UpdatedAt = DateTime.UtcNow;

        AddNotification(new GamePublishedDomainEvent(Id, Title.Value));
    }

    public void Unpublish()
    {
        if (Status != GameStatus.Published)
            throw new CatalogDomainException("Only published games can be unpublished.");

        Status = GameStatus.Unpublished;
        UpdatedAt = DateTime.UtcNow;

        AddNotification(new GameUnpublishedDomainEvent(Id, Title.Value));
    }

    public void Delete()
    {
        AddNotification(new GameDeletedDomainEvent(Id, Title.Value));
    }

    public void Archive()
    {
        Status = GameStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRatingStats(decimal averageRating, int totalReviews)
    {
        AverageRating = averageRating;
        TotalReviews = totalReviews;
        UpdatedAt = DateTime.UtcNow;
    }
}
