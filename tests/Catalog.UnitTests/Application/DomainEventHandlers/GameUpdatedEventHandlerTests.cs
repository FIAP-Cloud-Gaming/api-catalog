using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.DomainEventHandlers;

public sealed class GameUpdatedEventHandlerTests
{
    private readonly IGameRepository _gameRepository;
    private readonly IGameSearchRepository _gameSearchRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<GameUpdatedEventHandler> _logger;
    private readonly GameUpdatedEventHandler _handler;

    public GameUpdatedEventHandlerTests()
    {
        _gameRepository = Substitute.For<IGameRepository>();
        _gameSearchRepository = Substitute.For<IGameSearchRepository>();
        _eventBus = Substitute.For<IEventBus>();
        _logger = Substitute.For<ILogger<GameUpdatedEventHandler>>();
        _handler = new GameUpdatedEventHandler(
            _gameRepository,
            _gameSearchRepository,
            _eventBus,
            _logger
        );
    }

    [Fact]
    public async Task Handle_GameFound_ShouldUpdateSearchIndex()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGameWithId(1, "Updated Game", 59.99m);
        var notification = new GameUpdatedDomainEvent(game.Id, game.Title.Value);

        _gameRepository.GetByIdAsync(game.Id, Arg.Any<CancellationToken>())
            .Returns(game);
        _gameSearchRepository.UpdateGameAsync(Arg.Any<GameSearchDocument>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await _gameRepository.Received(1).GetByIdAsync(game.Id, Arg.Any<CancellationToken>());
        await _gameSearchRepository.Received(1).UpdateGameAsync(
            Arg.Is<GameSearchDocument>(doc => doc.Id == game.Id && doc.Title == game.Title.Value),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task Handle_GameNotFound_ShouldReturnWithoutError()
    {
        // Arrange
        var notification = new GameUpdatedDomainEvent(999, "Test Game");

        _gameRepository.GetByIdAsync(999, Arg.Any<CancellationToken>())
            .Returns((Game?)null);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await _gameRepository.Received(1).GetByIdAsync(999, Arg.Any<CancellationToken>());
        await _gameSearchRepository.DidNotReceive().UpdateGameAsync(Arg.Any<GameSearchDocument>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SearchFails_ShouldNotThrow()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGameWithId(1, "Updated Game", 59.99m);
        var notification = new GameUpdatedDomainEvent(game.Id, game.Title.Value);

        _gameRepository.GetByIdAsync(game.Id, Arg.Any<CancellationToken>())
            .Returns(game);
        _gameSearchRepository.UpdateGameAsync(Arg.Any<GameSearchDocument>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("Search service unavailable"));

        // Act
        var act = async () => await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        await _gameRepository.Received(1).GetByIdAsync(game.Id, Arg.Any<CancellationToken>());
    }
}
