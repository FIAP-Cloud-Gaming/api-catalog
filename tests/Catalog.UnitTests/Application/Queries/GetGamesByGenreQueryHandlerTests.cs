using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.Queries;

public sealed class GetGamesByGenreQueryHandlerTests
{
    private readonly IGameRepository _gameRepository;
    private readonly GetGamesByGenreQueryHandler _handler;

    public GetGamesByGenreQueryHandlerTests()
    {
        _gameRepository = Substitute.For<IGameRepository>();
        _handler = new GetGamesByGenreQueryHandler(_gameRepository);
    }

    [Fact]
    public async Task Handle_GamesExist_ShouldReturnViewModels()
    {
        // Arrange
        var game1 = GameTestFactory.CreateValidGame(title: "Dark Souls", genres: [Genre.RPG]);
        var game2 = GameTestFactory.CreateValidGame(title: "Elden Ring", genres: [Genre.RPG]);
        GameTestFactory.SetGameId(game1, 1);
        GameTestFactory.SetGameId(game2, 2);

        _gameRepository.GetByGenreAsync(Genre.RPG, Arg.Any<CancellationToken>())
            .Returns([game1, game2]);

        var query = new GetGamesByGenreQuery(Genre.RPG);

        // Act
        var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

        // Assert
        result.Should().HaveCount(2);
        result[0].Title.Should().Be("Dark Souls");
        result[1].Title.Should().Be("Elden Ring");
    }

    [Fact]
    public async Task Handle_NoGames_ShouldReturnEmpty()
    {
        // Arrange
        _gameRepository.GetByGenreAsync(Genre.Sports, Arg.Any<CancellationToken>())
            .Returns([]);

        var query = new GetGamesByGenreQuery(Genre.Sports);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldCallGetByGenreAsync()
    {
        // Arrange
        _gameRepository.GetByGenreAsync(Genre.Action, Arg.Any<CancellationToken>())
            .Returns([]);

        var query = new GetGamesByGenreQuery(Genre.Action);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        await _gameRepository.Received(1).GetByGenreAsync(Genre.Action, Arg.Any<CancellationToken>());
    }
}
