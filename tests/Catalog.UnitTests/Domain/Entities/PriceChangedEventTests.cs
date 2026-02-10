namespace Catalog.UnitTests.Domain.Entities;

public sealed class PriceChangedEventTests
{
    [Fact]
    public void Constructor_ShouldSetAllProperties()
    {
        // Arrange
        var gameId = 1;
        var oldPrice = 59.99m;
        var newPrice = 49.99m;
        var currency = "BRL";
        var reason = "Promotional discount";
        var changedBy = "admin";
        var beforeCreation = DateTime.UtcNow;

        // Act
        var priceEvent = new PriceChangedEvent(gameId, oldPrice, newPrice, currency, reason, changedBy);

        // Assert
        priceEvent.GameId.Should().Be(gameId);
        priceEvent.OldPrice.Should().Be(oldPrice);
        priceEvent.NewPrice.Should().Be(newPrice);
        priceEvent.Currency.Should().Be(currency);
        priceEvent.Reason.Should().Be(reason);
        priceEvent.ChangedBy.Should().Be(changedBy);
        priceEvent.ChangedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        priceEvent.ChangedAt.Should().BeOnOrAfter(beforeCreation);
    }

    [Fact]
    public void Constructor_NullCurrency_ShouldDefaultToBRL()
    {
        // Arrange & Act
        var priceEvent = new PriceChangedEvent(
            gameId: 1,
            oldPrice: 59.99m,
            newPrice: 49.99m,
            currency: null!,
            reason: "Discount",
            changedBy: "admin");

        // Assert
        priceEvent.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Constructor_NullReason_ShouldDefaultToEmpty()
    {
        // Arrange & Act
        var priceEvent = new PriceChangedEvent(
            gameId: 1,
            oldPrice: 59.99m,
            newPrice: 49.99m,
            currency: "BRL",
            reason: null!,
            changedBy: "admin");

        // Assert
        priceEvent.Reason.Should().Be(string.Empty);
    }

    [Fact]
    public void Constructor_NullChangedBy_ShouldDefaultToSystem()
    {
        // Arrange & Act
        var priceEvent = new PriceChangedEvent(
            gameId: 1,
            oldPrice: 59.99m,
            newPrice: 49.99m,
            currency: "BRL",
            reason: "Discount",
            changedBy: null!);

        // Assert
        priceEvent.ChangedBy.Should().Be("system");
    }
}
