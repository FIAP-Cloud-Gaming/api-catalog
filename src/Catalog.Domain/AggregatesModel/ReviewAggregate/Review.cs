namespace Catalog.Domain.AggregatesModel.ReviewAggregate;

public class Review : EntityWithNotification, IAggregateRoot
{
    public int GameId { get; private set; }
    public string UserId { get; private set; }
    public Rating Rating { get; private set; }
    public string Comment { get; private set; }
    public GameAggregate.Platform Platform { get; private set; }
    public bool VerifiedPurchase { get; private set; }
    public int? HoursPlayed { get; private set; }

    public int HelpfulCount { get; private set; }
    public int ReportedCount { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    protected Review()
    {
        UserId = string.Empty;
        Rating = null!;
        Comment = string.Empty;
    }

    private Review(
        int gameId,
        string userId,
        Rating rating,
        string comment,
        GameAggregate.Platform platform,
        bool verifiedPurchase,
        int? hoursPlayed)
    {
        GameId = gameId;
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        Rating = rating ?? throw new ArgumentNullException(nameof(rating));
        Comment = comment ?? string.Empty;
        Platform = platform;
        VerifiedPurchase = verifiedPurchase;
        HoursPlayed = hoursPlayed;
        CreatedAt = DateTime.UtcNow;
        HelpfulCount = 0;
        ReportedCount = 0;
    }

    public static Review Create(
        int gameId,
        string userId,
        Rating rating,
        string comment,
        GameAggregate.Platform platform,
        bool verifiedPurchase = false,
        int? hoursPlayed = null)
    {
        if (string.IsNullOrWhiteSpace(comment))
            throw new Exceptions.CatalogDomainException("Review comment cannot be empty.");

        if (comment.Length > 2000)
            throw new Exceptions.CatalogDomainException("Review comment cannot exceed 2000 characters.");

        var review = new Review(gameId, userId, rating, comment, platform, verifiedPurchase, hoursPlayed);
        review.AddNotification(new ReviewAddedDomainEvent(gameId, userId, rating.Value));

        return review;
    }

    public void UpdateComment(string newComment)
    {
        if (string.IsNullOrWhiteSpace(newComment))
            throw new Exceptions.CatalogDomainException("Review comment cannot be empty.");

        if (newComment.Length > 2000)
            throw new Exceptions.CatalogDomainException("Review comment cannot exceed 2000 characters.");

        Comment = newComment;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsHelpful()
    {
        HelpfulCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Report()
    {
        ReportedCount++;
        UpdatedAt = DateTime.UtcNow;
    }
}
