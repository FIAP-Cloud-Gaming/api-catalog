namespace Catalog.UnitTests.Domain.Models;

public sealed class TopGamesCacheTests
{
    [Fact]
    public void GetCacheKey_ShouldReturnCorrectKey()
    {
        // Arrange & Act
        var cacheKey = TopGamesCache.GetCacheKey();

        // Assert
        cacheKey.Should().Be("catalog:top-games");
    }

    [Fact]
    public void DefaultProperties_ShouldBeInitialized()
    {
        // Arrange & Act
        var topGamesCache = new TopGamesCache();

        // Assert
        topGamesCache.Games.Should().NotBeNull();
        topGamesCache.Games.Should().BeEmpty();
        topGamesCache.TotalGames.Should().Be(0);
        topGamesCache.CachedAt.Should().Be(default);
    }

    [Fact]
    public void Games_ShouldAllowAddingEntries()
    {
        // Arrange
        var topGamesCache = new TopGamesCache();
        var gameEntry = new TopGameEntry
        {
            GameId = 1,
            Title = "The Witcher 3",
            Score = 95.5m,
            Price = 59.99m,
            AverageRating = 4.8m
        };

        // Act
        topGamesCache.Games.Add(gameEntry);

        // Assert
        topGamesCache.Games.Should().HaveCount(1);
        topGamesCache.Games.First().GameId.Should().Be(1);
        topGamesCache.Games.First().Title.Should().Be("The Witcher 3");
    }

    [Fact]
    public void CachedAt_ShouldBeSettable()
    {
        // Arrange
        var topGamesCache = new TopGamesCache();
        var cachedTime = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        // Act
        topGamesCache.CachedAt = cachedTime;

        // Assert
        topGamesCache.CachedAt.Should().Be(cachedTime);
    }

    [Fact]
    public void TotalGames_ShouldBeSettable()
    {
        // Arrange
        var topGamesCache = new TopGamesCache();

        // Act
        topGamesCache.TotalGames = 150;

        // Assert
        topGamesCache.TotalGames.Should().Be(150);
    }

    [Fact]
    public void TopGameEntry_ShouldHaveAllProperties()
    {
        // Arrange & Act
        var entry = new TopGameEntry
        {
            GameId = 10,
            Title = "Cyberpunk 2077",
            Score = 88.5m,
            Price = 49.99m,
            AverageRating = 4.2m
        };

        // Assert
        entry.GameId.Should().Be(10);
        entry.Title.Should().Be("Cyberpunk 2077");
        entry.Score.Should().Be(88.5m);
        entry.Price.Should().Be(49.99m);
        entry.AverageRating.Should().Be(4.2m);
    }

    [Fact]
    public void TopGameEntry_DefaultConstructor_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var entry = new TopGameEntry();

        // Assert
        entry.GameId.Should().Be(0);
        entry.Title.Should().Be(string.Empty);
        entry.Score.Should().Be(0m);
        entry.Price.Should().Be(0m);
        entry.AverageRating.Should().Be(0m);
    }
}
