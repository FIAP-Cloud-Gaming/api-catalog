namespace Catalog.UnitTests.Domain.Events;

public sealed class DomainEventsTests
{
    [Fact]
    public void GameCreatedDomainEvent_ShouldImplementINotification()
    {
        // Arrange & Act
        var domainEvent = new GameCreatedDomainEvent(GameId: 1, Title: "Test Game");

        // Assert
        domainEvent.Should().BeAssignableTo<INotification>();
    }

    [Fact]
    public void GamePublishedDomainEvent_ShouldImplementINotification()
    {
        // Arrange & Act
        var domainEvent = new GamePublishedDomainEvent(GameId: 1, Title: "Test Game");

        // Assert
        domainEvent.Should().BeAssignableTo<INotification>();
    }

    [Fact]
    public void GameUpdatedDomainEvent_ShouldImplementINotification()
    {
        // Arrange & Act
        var domainEvent = new GameUpdatedDomainEvent(GameId: 1, Title: "Test Game");

        // Assert
        domainEvent.Should().BeAssignableTo<INotification>();
    }

    [Fact]
    public void GameDeletedDomainEvent_ShouldImplementINotification()
    {
        // Arrange & Act
        var domainEvent = new GameDeletedDomainEvent(GameId: 1, Title: "Test Game");

        // Assert
        domainEvent.Should().BeAssignableTo<INotification>();
    }

    [Fact]
    public void GameUnpublishedDomainEvent_ShouldImplementINotification()
    {
        // Arrange & Act
        var domainEvent = new GameUnpublishedDomainEvent(GameId: 1, Title: "Test Game");

        // Assert
        domainEvent.Should().BeAssignableTo<INotification>();
    }

    [Fact]
    public void GamePriceChangedDomainEvent_ShouldImplementINotification()
    {
        // Arrange
        var oldPrice = new Price(59.99m);
        var newPrice = new Price(49.99m);

        // Act
        var domainEvent = new GamePriceChangedDomainEvent(GameId: 1, OldPrice: oldPrice, NewPrice: newPrice);

        // Assert
        domainEvent.Should().BeAssignableTo<INotification>();
    }

    [Fact]
    public void ReviewAddedDomainEvent_ShouldImplementINotification()
    {
        // Arrange & Act
        var domainEvent = new ReviewAddedDomainEvent(GameId: 1, UserId: "user-123", Rating: 4);

        // Assert
        domainEvent.Should().BeAssignableTo<INotification>();
    }

    [Fact]
    public void PriceHistoryRecordedDomainEvent_ShouldImplementINotification()
    {
        // Arrange & Act
        var domainEvent = new PriceHistoryRecordedDomainEvent(GameId: 1, NewPrice: 49.99m);

        // Assert
        domainEvent.Should().BeAssignableTo<INotification>();
    }

    [Fact]
    public void GameCreatedDomainEvent_ShouldHaveCorrectProperties()
    {
        // Arrange
        var gameId = 123;
        var title = "The Witcher 3";

        // Act
        var domainEvent = new GameCreatedDomainEvent(gameId, title);

        // Assert
        domainEvent.GameId.Should().Be(gameId);
        domainEvent.Title.Should().Be(title);
    }

    [Fact]
    public void GamePriceChangedDomainEvent_ShouldHaveCorrectProperties()
    {
        // Arrange
        var gameId = 123;
        var oldPrice = new Price(59.99m, "BRL");
        var newPrice = new Price(49.99m, "BRL");

        // Act
        var domainEvent = new GamePriceChangedDomainEvent(gameId, oldPrice, newPrice);

        // Assert
        domainEvent.GameId.Should().Be(gameId);
        domainEvent.OldPrice.Should().Be(oldPrice);
        domainEvent.NewPrice.Should().Be(newPrice);
    }

    [Fact]
    public void ReviewAddedDomainEvent_ShouldHaveCorrectProperties()
    {
        // Arrange
        var gameId = 123;
        var userId = "user-456";
        var rating = 5;

        // Act
        var domainEvent = new ReviewAddedDomainEvent(gameId, userId, rating);

        // Assert
        domainEvent.GameId.Should().Be(gameId);
        domainEvent.UserId.Should().Be(userId);
        domainEvent.Rating.Should().Be(rating);
    }

    [Fact]
    public void RecordEquality_ShouldWorkCorrectly()
    {
        // Arrange
        var event1 = new GameCreatedDomainEvent(1, "Test Game");
        var event2 = new GameCreatedDomainEvent(1, "Test Game");

        // Act & Assert
        event1.Should().Be(event2);
        (event1 == event2).Should().BeTrue();
    }

    [Fact]
    public void RecordInequality_ShouldWorkCorrectly()
    {
        // Arrange
        var event1 = new GameCreatedDomainEvent(1, "Test Game");
        var event2 = new GameCreatedDomainEvent(2, "Different Game");

        // Act & Assert
        event1.Should().NotBe(event2);
        (event1 != event2).Should().BeTrue();
    }
}
