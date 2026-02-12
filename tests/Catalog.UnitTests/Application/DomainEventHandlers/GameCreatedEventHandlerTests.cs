using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.DomainEventHandlers;

public sealed class GameCreatedEventHandlerTests
{
    private readonly IGameRepository _gameRepository;
    private readonly IGameSearchRepository _gameSearchRepository;
    private readonly IPriceHistoryRepository _priceHistoryRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<GameCreatedEventHandler> _logger;
    private readonly GameCreatedEventHandler _handler;

    public GameCreatedEventHandlerTests()
    {
        _gameRepository = Substitute.For<IGameRepository>();
        _gameSearchRepository = Substitute.For<IGameSearchRepository>();
        _priceHistoryRepository = Substitute.For<IPriceHistoryRepository>();
        _eventBus = Substitute.For<IEventBus>();
        _logger = Substitute.For<ILogger<GameCreatedEventHandler>>();
        _handler = new GameCreatedEventHandler(
            _gameRepository,
            _gameSearchRepository,
            _priceHistoryRepository,
            _eventBus,
            _logger
        );
    }

    [Fact]
    public async Task Handle_GameFound_ShouldIndexAndCreateHistory()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGameWithId(1, "Test Game", 59.99m);
        var notification = new GameCreatedDomainEvent(game.Id, game.Title.Value);

        _gameRepository.GetByIdAsync(game.Id, Arg.Any<CancellationToken>())
            .Returns(game);
        _gameSearchRepository.IndexGameAsync(Arg.Any<GameSearchDocument>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        _priceHistoryRepository.AddAsync(Arg.Any<GamePriceHistory>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await _gameRepository.Received(1).GetByIdAsync(game.Id, Arg.Any<CancellationToken>());
        await _gameSearchRepository.Received(1).IndexGameAsync(
            Arg.Is<GameSearchDocument>(doc => doc.Id == game.Id && doc.Title == game.Title.Value),
            Arg.Any<CancellationToken>()
        );
        await _priceHistoryRepository.Received(1).AddAsync(
            Arg.Is<GamePriceHistory>(ph => ph.GameId == game.Id),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task Handle_GameNotFound_ShouldReturnWithoutError()
    {
        // Arrange
        var notification = new GameCreatedDomainEvent(999, "Test Game");

        _gameRepository.GetByIdAsync(999, Arg.Any<CancellationToken>())
            .Returns((Game?)null);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await _gameRepository.Received(1).GetByIdAsync(999, Arg.Any<CancellationToken>());
        await _gameSearchRepository.DidNotReceive().IndexGameAsync(Arg.Any<GameSearchDocument>(), Arg.Any<CancellationToken>());
        await _priceHistoryRepository.DidNotReceive().AddAsync(Arg.Any<GamePriceHistory>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_IndexFails_ShouldNotThrow()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGameWithId(1, "Test Game", 59.99m);
        var notification = new GameCreatedDomainEvent(game.Id, game.Title.Value);

        _gameRepository.GetByIdAsync(game.Id, Arg.Any<CancellationToken>())
            .Returns(game);
        _gameSearchRepository.IndexGameAsync(Arg.Any<GameSearchDocument>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("Search service unavailable"));

        // Act
        var act = async () => await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        await _gameRepository.Received(1).GetByIdAsync(game.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldCallIndexGameAsync()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGameWithId(1, "Test Game", 59.99m);
        var notification = new GameCreatedDomainEvent(game.Id, game.Title.Value);

        _gameRepository.GetByIdAsync(game.Id, Arg.Any<CancellationToken>())
            .Returns(game);
        _gameSearchRepository.IndexGameAsync(Arg.Any<GameSearchDocument>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        _priceHistoryRepository.AddAsync(Arg.Any<GamePriceHistory>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await _gameSearchRepository.Received(1).IndexGameAsync(
            Arg.Is<GameSearchDocument>(doc => doc.Id == game.Id && doc.Title == game.Title.Value),
            Arg.Any<CancellationToken>()
        );
    }
}
