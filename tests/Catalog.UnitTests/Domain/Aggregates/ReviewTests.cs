using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Domain.Aggregates;

public sealed class ReviewTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateReview()
    {
        // Arrange
        var gameId = 1;
        var userId = "user-123";
        var rating = new Rating(4);
        var comment = "Great game!";
        var platform = Platform.PC;

        // Act
        var review = Review.Create(gameId, userId, rating, comment, platform, verifiedPurchase: true, hoursPlayed: 10);

        // Assert
        review.Should().NotBeNull();
        review.GameId.Should().Be(gameId);
        review.UserId.Should().Be(userId);
        review.Rating.Value.Should().Be(4);
        review.Comment.Should().Be(comment);
        review.Platform.Should().Be(platform);
        review.VerifiedPurchase.Should().BeTrue();
        review.HoursPlayed.Should().Be(10);
        review.HelpfulCount.Should().Be(0);
        review.ReportedCount.Should().Be(0);
        review.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ShouldRaiseReviewAddedDomainEvent()
    {
        // Arrange
        var gameId = 1;
        var userId = "user-123";
        var rating = new Rating(4);
        var comment = "Great game!";

        // Act
        var review = Review.Create(gameId, userId, rating, comment, Platform.PC);

        // Assert
        var domainEvents = review.Notifications;
        domainEvents.Should().HaveCount(1);
        domainEvents.First().Should().BeOfType<ReviewAddedDomainEvent>();
        var addedEvent = (ReviewAddedDomainEvent)domainEvents.First();
        addedEvent.GameId.Should().Be(gameId);
        addedEvent.UserId.Should().Be(userId);
        addedEvent.Rating.Should().Be(4);
    }

    [Fact]
    public void Create_WithEmptyComment_ShouldThrowCatalogDomainException()
    {
        // Arrange
        var gameId = 1;
        var userId = "user-123";
        var rating = new Rating(4);
        var emptyComment = "";

        // Act
        var act = () => Review.Create(gameId, userId, rating, emptyComment, Platform.PC);

        // Assert
        act.Should().Throw<CatalogDomainException>()
            .WithMessage("Review comment cannot be empty.");
    }

    [Fact]
    public void Create_WithCommentTooLong_ShouldThrowCatalogDomainException()
    {
        // Arrange
        var gameId = 1;
        var userId = "user-123";
        var rating = new Rating(4);
        var longComment = new string('a', 2001);

        // Act
        var act = () => Review.Create(gameId, userId, rating, longComment, Platform.PC);

        // Assert
        act.Should().Throw<CatalogDomainException>()
            .WithMessage("Review comment cannot exceed 2000 characters.");
    }

    [Fact]
    public void Create_WithNullUserId_ShouldThrowArgumentNullException()
    {
        // Arrange
        var gameId = 1;
        var rating = new Rating(4);
        var comment = "Great game!";

        // Act
        var act = () => Review.Create(gameId, null!, rating, comment, Platform.PC);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("userId");
    }

    [Fact]
    public void Create_WithNullRating_ShouldThrowArgumentNullException()
    {
        // Arrange
        var gameId = 1;
        var userId = "user-123";
        var comment = "Great game!";

        // Act
        var act = () => Review.Create(gameId, userId, null!, comment, Platform.PC);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("rating");
    }

    [Fact]
    public void UpdateComment_WithValidComment_ShouldUpdate()
    {
        // Arrange
        var review = GameTestFactory.CreateValidReview(comment: "Old comment");
        var newComment = "Updated comment";

        // Act
        review.UpdateComment(newComment);

        // Assert
        review.Comment.Should().Be(newComment);
    }

    [Fact]
    public void UpdateComment_WithEmptyComment_ShouldThrow()
    {
        // Arrange
        var review = GameTestFactory.CreateValidReview();

        // Act
        var act = () => review.UpdateComment("");

        // Assert
        act.Should().Throw<CatalogDomainException>()
            .WithMessage("Review comment cannot be empty.");
    }

    [Fact]
    public void UpdateComment_WithCommentTooLong_ShouldThrow()
    {
        // Arrange
        var review = GameTestFactory.CreateValidReview();
        var longComment = new string('a', 2001);

        // Act
        var act = () => review.UpdateComment(longComment);

        // Assert
        act.Should().Throw<CatalogDomainException>()
            .WithMessage("Review comment cannot exceed 2000 characters.");
    }

    [Fact]
    public void UpdateComment_ShouldSetUpdatedAt()
    {
        // Arrange
        var review = GameTestFactory.CreateValidReview();
        var beforeUpdate = DateTime.UtcNow;

        // Act
        review.UpdateComment("Updated comment");

        // Assert
        review.UpdatedAt.Should().NotBeNull();
        review.UpdatedAt.Should().BeOnOrAfter(beforeUpdate);
    }

    [Fact]
    public void MarkAsHelpful_ShouldIncrementHelpfulCount()
    {
        // Arrange
        var review = GameTestFactory.CreateValidReview();
        review.HelpfulCount.Should().Be(0);

        // Act
        review.MarkAsHelpful();

        // Assert
        review.HelpfulCount.Should().Be(1);
    }

    [Fact]
    public void Report_ShouldIncrementReportedCount()
    {
        // Arrange
        var review = GameTestFactory.CreateValidReview();
        review.ReportedCount.Should().Be(0);

        // Act
        review.Report();

        // Assert
        review.ReportedCount.Should().Be(1);
    }

    [Fact]
    public void MarkAsHelpful_CalledMultipleTimes_ShouldIncrementCorrectly()
    {
        // Arrange
        var review = GameTestFactory.CreateValidReview();

        // Act
        review.MarkAsHelpful();
        review.MarkAsHelpful();
        review.MarkAsHelpful();

        // Assert
        review.HelpfulCount.Should().Be(3);
    }
}
