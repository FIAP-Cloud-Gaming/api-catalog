using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.Queries;

public sealed class GetGameByIdQueryHandlerTests
{
    private readonly IGameRepository _gameRepository;
    private readonly GetGameByIdQueryHandler _handler;

    public GetGameByIdQueryHandlerTests()
    {
        _gameRepository = Substitute.For<IGameRepository>();
        _handler = new GetGameByIdQueryHandler(_gameRepository);
    }

    [Fact]
    public async Task Handle_GameExists_ShouldReturnGameViewModel()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGameWithId(1, "Elden Ring", 199.99m);
        _gameRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(game);

        var query = new GetGameByIdQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Title.Should().Be("Elden Ring");
        result.PriceAmount.Should().Be(199.99m);
    }

    [Fact]
    public async Task Handle_GameNotExists_ShouldReturnNull()
    {
        // Arrange
        _gameRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Game?)null);

        var query = new GetGameByIdQuery(999);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldMapAllProperties()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGame(
            title: "God of War",
            description: "An epic adventure",
            priceAmount: 249.90m,
            priceCurrency: "BRL",
            genres: [Genre.Action, Genre.Adventure],
            platforms: [Platform.PlayStation5],
            releaseDate: new DateTime(2022, 11, 9),
            publisher: "Sony Interactive",
            developer: "Santa Monica Studio",
            coverImageUrl: "https://example.com/gow.jpg");

        GameTestFactory.SetGameId(game, 1);

        _gameRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(game);

        var query = new GetGameByIdQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Title.Should().Be("God of War");
        result.Description.Should().Be("An epic adventure");
        result.PriceAmount.Should().Be(249.90m);
        result.PriceCurrency.Should().Be("BRL");
        result.Genres.Should().HaveCount(2);
        result.Platforms.Should().HaveCount(1);
        result.ReleaseDate.Should().Be(new DateTime(2022, 11, 9));
        result.Publisher.Should().Be("Sony Interactive");
        result.Developer.Should().Be("Santa Monica Studio");
        result.CoverImageUrl.Should().Be("https://example.com/gow.jpg");
    }
}
