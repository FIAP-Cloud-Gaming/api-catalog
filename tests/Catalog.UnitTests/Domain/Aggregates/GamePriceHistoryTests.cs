using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Domain.Aggregates;

public sealed class GamePriceHistoryTests
{
    [Fact]
    public void Constructor_ShouldSetGameId()
    {
        // Arrange
        var gameId = 123;

        // Act
        var history = new GamePriceHistory(gameId);

        // Assert
        history.GameId.Should().Be(gameId);
        history.Events.Should().BeEmpty();
    }

    [Fact]
    public void RecordPriceChange_ShouldAddEvent()
    {
        // Arrange
        var history = new GamePriceHistory(gameId: 1);

        // Act
        history.RecordPriceChange(
            oldPrice: 59.99m,
            newPrice: 49.99m,
            currency: "BRL",
            reason: "Promotional discount",
            changedBy: "admin");

        // Assert
        history.Events.Should().HaveCount(1);
        var priceEvent = history.Events.First();
        priceEvent.GameId.Should().Be(1);
        priceEvent.OldPrice.Should().Be(59.99m);
        priceEvent.NewPrice.Should().Be(49.99m);
        priceEvent.Currency.Should().Be("BRL");
        priceEvent.Reason.Should().Be("Promotional discount");
        priceEvent.ChangedBy.Should().Be("admin");
    }

    [Fact]
    public void RecordPriceChange_ShouldRaisePriceHistoryRecordedDomainEvent()
    {
        // Arrange
        var history = new GamePriceHistory(gameId: 1);

        // Act
        history.RecordPriceChange(59.99m, 49.99m, "BRL", "Discount", "admin");

        // Assert
        var domainEvents = history.Notifications;
        domainEvents.Should().HaveCount(1);
        domainEvents.First().Should().BeOfType<PriceHistoryRecordedDomainEvent>();
        var recordedEvent = (PriceHistoryRecordedDomainEvent)domainEvents.First();
        recordedEvent.GameId.Should().Be(1);
        recordedEvent.NewPrice.Should().Be(49.99m);
    }

    [Fact]
    public void GetCurrentPrice_WhenNoEvents_ShouldReturnZero()
    {
        // Arrange
        var history = new GamePriceHistory(gameId: 1);

        // Act
        var currentPrice = history.GetCurrentPrice();

        // Assert
        currentPrice.Should().Be(0);
    }

    [Fact]
    public void GetCurrentPrice_ShouldReturnLastEventNewPrice()
    {
        // Arrange
        var history = new GamePriceHistory(gameId: 1);
        history.RecordPriceChange(59.99m, 49.99m, "BRL", "First discount", "admin");
        history.RecordPriceChange(49.99m, 39.99m, "BRL", "Second discount", "admin");
        history.RecordPriceChange(39.99m, 44.99m, "BRL", "Price increase", "admin");

        // Act
        var currentPrice = history.GetCurrentPrice();

        // Assert
        currentPrice.Should().Be(44.99m);
    }

    [Fact]
    public void GetHistory_ShouldReturnEventsOrderedByDescDate()
    {
        // Arrange
        var history = new GamePriceHistory(gameId: 1);

        history.RecordPriceChange(59.99m, 49.99m, "BRL", "First", "admin");
        System.Threading.Thread.Sleep(10);
        history.RecordPriceChange(49.99m, 39.99m, "BRL", "Second", "admin");
        System.Threading.Thread.Sleep(10);
        history.RecordPriceChange(39.99m, 44.99m, "BRL", "Third", "admin");

        // Act
        var orderedHistory = history.GetHistory().ToList();

        // Assert
        orderedHistory.Should().HaveCount(3);
        orderedHistory[0].Reason.Should().Be("Third");
        orderedHistory[1].Reason.Should().Be("Second");
        orderedHistory[2].Reason.Should().Be("First");
    }

    [Fact]
    public void GetHistoryInPeriod_ShouldFilterByDateRange()
    {
        // Arrange
        var history = new GamePriceHistory(gameId: 1);

        var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var middleDate = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        history.RecordPriceChange(59.99m, 49.99m, "BRL", "January change", "admin");

        // Act
        var filteredHistory = history.GetHistoryInPeriod(middleDate, endDate).ToList();

        // Assert
        filteredHistory.Should().BeEmpty();
    }

    [Fact]
    public void GetHistoryInPeriod_NoMatchingEvents_ShouldReturnEmpty()
    {
        // Arrange
        var history = new GamePriceHistory(gameId: 1);
        history.RecordPriceChange(59.99m, 49.99m, "BRL", "Change", "admin");

        var futureStartDate = DateTime.UtcNow.AddDays(10);
        var futureEndDate = DateTime.UtcNow.AddDays(20);

        // Act
        var filteredHistory = history.GetHistoryInPeriod(futureStartDate, futureEndDate);

        // Assert
        filteredHistory.Should().BeEmpty();
    }
}
