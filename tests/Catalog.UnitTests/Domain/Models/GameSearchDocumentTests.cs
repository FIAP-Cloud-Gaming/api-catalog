namespace Catalog.UnitTests.Domain.Models;

public sealed class GameSearchDocumentTests
{
    [Fact]
    public void FromGame_ShouldMapAllProperties()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGameWithId(
            id: 123,
            title: "The Witcher 3 Wild Hunt",
            priceAmount: 99.90m);

        game.UpdateRatingStats(averageRating: 4.9m, totalReviews: 500);
        game.Publish();

        // Act
        var searchDocument = GameSearchDocument.FromGame(game);

        // Assert
        searchDocument.Should().NotBeNull();
        searchDocument.Id.Should().Be(123);
        searchDocument.Title.Should().Be("The Witcher 3 Wild Hunt");
        searchDocument.Description.Should().NotBeNull();
        searchDocument.Price.Should().Be(99.90m);
        searchDocument.Currency.Should().Be("BRL");
        searchDocument.Genres.Should().NotBeEmpty();
        searchDocument.Platforms.Should().NotBeEmpty();
        searchDocument.Status.Should().Be("Published");
        searchDocument.AverageRating.Should().Be(4.9m);
        searchDocument.TotalReviews.Should().Be(500);
        searchDocument.IndexedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void FromGame_ShouldGenerateTagsFromTitle()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGameWithId(
            id: 1,
            title: "The Last of Us Part Two");

        // Act
        var searchDocument = GameSearchDocument.FromGame(game);

        // Assert
        searchDocument.Tags.Should().Contain("the");
        searchDocument.Tags.Should().Contain("last");
        searchDocument.Tags.Should().Contain("of");
        searchDocument.Tags.Should().Contain("us");
        searchDocument.Tags.Should().Contain("part");
        searchDocument.Tags.Should().Contain("two");
    }

    [Fact]
    public void FromGame_ShouldIncludeGenresAndPlatformsInTags()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame(
            title: "TestGame",
            genres: new[] { Genre.Action, Genre.Adventure },
            platforms: new[] { Platform.PC, Platform.PlayStation5 });

        GameTestFactory.SetGameId(game, 1);

        // Act
        var searchDocument = GameSearchDocument.FromGame(game);

        // Assert
        searchDocument.Tags.Should().Contain("action");
        searchDocument.Tags.Should().Contain("adventure");
        searchDocument.Tags.Should().Contain("pc");
        searchDocument.Tags.Should().Contain("playstation5");
    }

    [Fact]
    public void FromGame_Tags_ShouldBeDistinctAndLowercase()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame(
            title: "Action Game Action",
            genres: new[] { Genre.Action },
            platforms: new[] { Platform.PC });

        GameTestFactory.SetGameId(game, 1);

        // Act
        var searchDocument = GameSearchDocument.FromGame(game);

        // Assert
        searchDocument.Tags.Should().OnlyContain(tag => tag == tag.ToLower(), "all tags should be lowercase");
        searchDocument.Tags.Should().OnlyHaveUniqueItems("tags should be distinct");
        searchDocument.Tags.Where(t => t == "action").Should().HaveCount(1, "duplicate 'action' should be removed");
    }
}
