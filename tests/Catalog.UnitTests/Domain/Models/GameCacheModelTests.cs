namespace Catalog.UnitTests.Domain.Models;

public sealed class GameCacheModelTests
{
    [Fact]
    public void FromGame_ShouldMapAllProperties()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGameWithId(
            id: 123,
            title: "The Last of Us",
            priceAmount: 59.99m);

        game.UpdateRatingStats(averageRating: 4.8m, totalReviews: 150);

        // Act
        var cacheModel = GameCacheModel.FromGame(game);

        // Assert
        cacheModel.Should().NotBeNull();
        cacheModel.Id.Should().Be(123);
        cacheModel.Title.Should().Be("The Last of Us");
        cacheModel.Price.Should().Be(59.99m);
        cacheModel.Currency.Should().Be("BRL");
        cacheModel.AverageRating.Should().Be(4.8m);
        cacheModel.TotalReviews.Should().Be(150);
        cacheModel.CoverImageUrl.Should().NotBeNull();
        cacheModel.CachedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void GetCacheKey_ShouldReturnCorrectFormat()
    {
        // Arrange
        var cacheModel = new GameCacheModel { Id = 456 };

        // Act
        var cacheKey = cacheModel.GetCacheKey();

        // Assert
        cacheKey.Should().Be("catalog:game:456");
    }

    [Fact]
    public void DefaultCurrency_ShouldBeBRL()
    {
        // Arrange & Act
        var cacheModel = new GameCacheModel();

        // Assert
        cacheModel.Currency.Should().Be("BRL");
    }

    [Fact]
    public void CachedAt_ShouldBeSetToUtcNow()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGameWithId(id: 1);
        var beforeCaching = DateTime.UtcNow;

        // Act
        var cacheModel = GameCacheModel.FromGame(game);

        // Assert
        cacheModel.CachedAt.Should().BeOnOrAfter(beforeCaching);
        cacheModel.CachedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
