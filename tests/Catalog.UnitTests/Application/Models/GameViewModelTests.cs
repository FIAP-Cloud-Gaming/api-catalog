using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.Models;

public sealed class GameViewModelTests
{
    [Fact]
    public void FromGame_ShouldMapAllProperties()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame(
            title: "Test Game",
            description: "Test Description",
            priceAmount: 59.99m,
            priceCurrency: "BRL",
            genres: [Genre.Action, Genre.Adventure],
            platforms: [Platform.PC, Platform.PlayStation5],
            releaseDate: new DateTime(2024, 12, 31),
            publisher: "Test Publisher",
            developer: "Test Developer",
            coverImageUrl: "https://example.com/cover.jpg"
        );

        GameTestFactory.SetGameId(game, 1);
        game.Publish();

        // Act
        var viewModel = GameViewModel.FromGame(game);

        // Assert
        viewModel.Id.Should().Be(1);
        viewModel.Title.Should().Be("Test Game");
        viewModel.Description.Should().Be("Test Description");
        viewModel.PriceAmount.Should().Be(59.99m);
        viewModel.PriceCurrency.Should().Be("BRL");
        viewModel.Status.Should().Be(GameStatus.Published);
        viewModel.Genres.Should().BeEquivalentTo([Genre.Action, Genre.Adventure]);
        viewModel.Platforms.Should().BeEquivalentTo([Platform.PC, Platform.PlayStation5]);
        viewModel.ReleaseDate.Should().Be(new DateTime(2024, 12, 31));
        viewModel.Publisher.Should().Be("Test Publisher");
        viewModel.Developer.Should().Be("Test Developer");
        viewModel.CoverImageUrl.Should().Be("https://example.com/cover.jpg");
        viewModel.AverageRating.Should().Be(0.0m);
        viewModel.TotalReviews.Should().Be(0);
        viewModel.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void FromGame_NullGame_ShouldThrowArgumentNullException()
    {
        // Arrange
        Game? game = null;

        // Act
        var act = () => GameViewModel.FromGame(game!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("game");
    }

    [Fact]
    public void FromGame_ShouldMapGenresAndPlatforms()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame(
            title: "Multi-Genre Game",
            description: "Description",
            priceAmount: 49.99m,
            priceCurrency: "USD",
            genres: [Genre.Action, Genre.RPG, Genre.Strategy],
            platforms: [Platform.PC, Platform.XboxSeriesX, Platform.PlayStation5, Platform.NintendoSwitch],
            releaseDate: DateTime.UtcNow,
            publisher: null,
            developer: null,
            coverImageUrl: null
        );

        GameTestFactory.SetGameId(game, 42);

        // Act
        var viewModel = GameViewModel.FromGame(game);

        // Assert
        viewModel.Genres.Should().HaveCount(3);
        viewModel.Genres.Should().Contain(Genre.Action);
        viewModel.Genres.Should().Contain(Genre.RPG);
        viewModel.Genres.Should().Contain(Genre.Strategy);

        viewModel.Platforms.Should().HaveCount(4);
        viewModel.Platforms.Should().Contain(Platform.PC);
        viewModel.Platforms.Should().Contain(Platform.XboxSeriesX);
        viewModel.Platforms.Should().Contain(Platform.PlayStation5);
        viewModel.Platforms.Should().Contain(Platform.NintendoSwitch);
    }
}
