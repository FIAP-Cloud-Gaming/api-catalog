using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.Queries;

public sealed class GetGamesByStatusQueryHandlerTests
{
    private readonly IGameRepository _gameRepository;
    private readonly GetGamesByStatusQueryHandler _handler;

    public GetGamesByStatusQueryHandlerTests()
    {
        _gameRepository = Substitute.For<IGameRepository>();
        _handler = new GetGamesByStatusQueryHandler(_gameRepository);
    }

    [Fact]
    public async Task Handle_GamesExist_ShouldReturnViewModels()
    {
        // Arrange
        var game1 = GameTestFactory.CreateValidGameWithId(1, "Published Game 1");
        var game2 = GameTestFactory.CreateValidGameWithId(2, "Published Game 2");
        game1.Publish();
        game2.Publish();

        _gameRepository.GetByStatusAsync(GameStatus.Published, Arg.Any<CancellationToken>())
            .Returns([game1, game2]);

        var query = new GetGamesByStatusQuery(GameStatus.Published);

        // Act
        var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

        // Assert
        result.Should().HaveCount(2);
        result[0].Status.Should().Be(GameStatus.Published);
        result[1].Status.Should().Be(GameStatus.Published);
    }

    [Fact]
    public async Task Handle_NoGames_ShouldReturnEmpty()
    {
        // Arrange
        _gameRepository.GetByStatusAsync(GameStatus.Archived, Arg.Any<CancellationToken>())
            .Returns([]);

        var query = new GetGamesByStatusQuery(GameStatus.Archived);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldCallGetByStatusAsync()
    {
        // Arrange
        _gameRepository.GetByStatusAsync(GameStatus.Draft, Arg.Any<CancellationToken>())
            .Returns([]);

        var query = new GetGamesByStatusQuery(GameStatus.Draft);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        await _gameRepository.Received(1).GetByStatusAsync(GameStatus.Draft, Arg.Any<CancellationToken>());
    }
}
