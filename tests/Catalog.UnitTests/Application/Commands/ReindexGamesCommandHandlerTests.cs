using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.Commands;

public sealed class ReindexGamesCommandHandlerTests
{
    private readonly IGameRepository _gameRepository;
    private readonly IGameSearchRepository _searchRepository;
    private readonly ILogger<ReindexGamesCommandHandler> _logger;
    private readonly ReindexGamesCommandHandler _handler;

    public ReindexGamesCommandHandlerTests()
    {
        _gameRepository = Substitute.For<IGameRepository>();
        _searchRepository = Substitute.For<IGameSearchRepository>();
        _logger = Substitute.For<ILogger<ReindexGamesCommandHandler>>();

        _handler = new ReindexGamesCommandHandler(_gameRepository, _searchRepository, _logger);
    }

    [Fact]
    public async Task Handle_AllGamesIndexedSuccessfully_ShouldReturnCorrectCounts()
    {
        // Arrange
        var game1 = GameTestFactory.CreateValidGameWithId(1, "Game 1");
        var game2 = GameTestFactory.CreateValidGameWithId(2, "Game 2");
        var game3 = GameTestFactory.CreateValidGameWithId(3, "Game 3");

        _gameRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns([game1, game2, game3]);

        _searchRepository.IndexGameAsync(Arg.Any<GameSearchDocument>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var command = new ReindexGamesCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalGames.Should().Be(3);
        result.IndexedGames.Should().Be(3);
        result.FailedGames.Should().Be(0);
    }

    [Fact]
    public async Task Handle_SomeGamesFail_ShouldCountFailures()
    {
        // Arrange
        var game1 = GameTestFactory.CreateValidGameWithId(1, "Game 1");
        var game2 = GameTestFactory.CreateValidGameWithId(2, "Game 2");
        var game3 = GameTestFactory.CreateValidGameWithId(3, "Game 3");

        _gameRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns([game1, game2, game3]);

        var callCount = 0;
        _searchRepository.IndexGameAsync(Arg.Any<GameSearchDocument>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                callCount++;
                if (callCount == 2)
                {
                    throw new Exception("Indexing failed");
                }
                return Task.CompletedTask;
            });

        var command = new ReindexGamesCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalGames.Should().Be(3);
        result.IndexedGames.Should().Be(2);
        result.FailedGames.Should().Be(1);
    }

    [Fact]
    public async Task Handle_NoGames_ShouldReturnZeroCounts()
    {
        // Arrange
        _gameRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns([]);

        var command = new ReindexGamesCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalGames.Should().Be(0);
        result.IndexedGames.Should().Be(0);
        result.FailedGames.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldCallIndexGameAsyncForEachGame()
    {
        // Arrange
        var game1 = GameTestFactory.CreateValidGameWithId(1, "Game 1");
        var game2 = GameTestFactory.CreateValidGameWithId(2, "Game 2");

        _gameRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns([game1, game2]);

        _searchRepository.IndexGameAsync(Arg.Any<GameSearchDocument>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var command = new ReindexGamesCommand();

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _searchRepository.Received(2).IndexGameAsync(
            Arg.Any<GameSearchDocument>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_FailedIndex_ShouldContinueProcessing()
    {
        // Arrange
        var game1 = GameTestFactory.CreateValidGameWithId(1, "Game 1");
        var game2 = GameTestFactory.CreateValidGameWithId(2, "Game 2");
        var game3 = GameTestFactory.CreateValidGameWithId(3, "Game 3");

        _gameRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns([game1, game2, game3]);

        _searchRepository.IndexGameAsync(
            Arg.Is<GameSearchDocument>(doc => doc.Id == 1),
            Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("First game failed"));

        _searchRepository.IndexGameAsync(
            Arg.Is<GameSearchDocument>(doc => doc.Id == 2 || doc.Id == 3),
            Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var command = new ReindexGamesCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.TotalGames.Should().Be(3);
        result.IndexedGames.Should().Be(2);
        result.FailedGames.Should().Be(1);
        await _searchRepository.Received(3).IndexGameAsync(
            Arg.Any<GameSearchDocument>(),
            Arg.Any<CancellationToken>());
    }
}
