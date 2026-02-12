using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.DomainEventHandlers;

public sealed class GamePriceChangedEventHandlerTests
{
    private readonly IGameRepository _gameRepository;
    private readonly IGameSearchRepository _gameSearchRepository;
    private readonly IPriceHistoryRepository _priceHistoryRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<GamePriceChangedEventHandler> _logger;
    private readonly GamePriceChangedEventHandler _handler;

    public GamePriceChangedEventHandlerTests()
    {
        _gameRepository = Substitute.For<IGameRepository>();
        _gameSearchRepository = Substitute.For<IGameSearchRepository>();
        _priceHistoryRepository = Substitute.For<IPriceHistoryRepository>();
        _eventBus = Substitute.For<IEventBus>();
        _logger = Substitute.For<ILogger<GamePriceChangedEventHandler>>();
        _handler = new GamePriceChangedEventHandler(
            _gameRepository,
            _gameSearchRepository,
            _priceHistoryRepository,
            _eventBus,
            _logger
        );
    }

    [Fact]
    public async Task Handle_GameFound_HistoryExists_ShouldUpdateHistory()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGameWithId(1, "Test Game", 59.99m);
        var oldPrice = new Price(49.99m, "BRL");
        var newPrice = game.Price;
        var notification = new GamePriceChangedDomainEvent(game.Id, oldPrice, newPrice);

        var existingHistory = new GamePriceHistory(game.Id);

        _gameRepository.GetByIdAsync(game.Id, Arg.Any<CancellationToken>())
            .Returns(game);
        _priceHistoryRepository.GetByGameIdAsync(game.Id, Arg.Any<CancellationToken>())
            .Returns(existingHistory);
        _gameSearchRepository.UpdateGameAsync(Arg.Any<GameSearchDocument>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        _priceHistoryRepository.UpdateAsync(Arg.Any<GamePriceHistory>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await _gameRepository.Received(1).GetByIdAsync(game.Id, Arg.Any<CancellationToken>());
        await _priceHistoryRepository.Received(1).GetByGameIdAsync(game.Id, Arg.Any<CancellationToken>());
        await _priceHistoryRepository.Received(1).UpdateAsync(Arg.Any<GamePriceHistory>(), Arg.Any<CancellationToken>());
        await _priceHistoryRepository.DidNotReceive().AddAsync(Arg.Any<GamePriceHistory>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_GameFound_HistoryNotExists_ShouldCreateHistory()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGameWithId(1, "Test Game", 59.99m);
        var oldPrice = new Price(49.99m, "BRL");
        var newPrice = game.Price;
        var notification = new GamePriceChangedDomainEvent(game.Id, oldPrice, newPrice);

        _gameRepository.GetByIdAsync(game.Id, Arg.Any<CancellationToken>())
            .Returns(game);
        _priceHistoryRepository.GetByGameIdAsync(game.Id, Arg.Any<CancellationToken>())
            .Returns((GamePriceHistory?)null);
        _gameSearchRepository.UpdateGameAsync(Arg.Any<GameSearchDocument>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        _priceHistoryRepository.AddAsync(Arg.Any<GamePriceHistory>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await _gameRepository.Received(1).GetByIdAsync(game.Id, Arg.Any<CancellationToken>());
        await _priceHistoryRepository.Received(1).GetByGameIdAsync(game.Id, Arg.Any<CancellationToken>());
        await _priceHistoryRepository.Received(1).AddAsync(
            Arg.Is<GamePriceHistory>(ph => ph.GameId == game.Id),
            Arg.Any<CancellationToken>()
        );
        await _priceHistoryRepository.DidNotReceive().UpdateAsync(Arg.Any<GamePriceHistory>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_GameNotFound_ShouldReturnWithoutError()
    {
        // Arrange
        var oldPrice = new Price(49.99m, "BRL");
        var newPrice = new Price(59.99m, "BRL");
        var notification = new GamePriceChangedDomainEvent(999, oldPrice, newPrice);

        _gameRepository.GetByIdAsync(999, Arg.Any<CancellationToken>())
            .Returns((Game?)null);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await _gameRepository.Received(1).GetByIdAsync(999, Arg.Any<CancellationToken>());
        await _gameSearchRepository.DidNotReceive().UpdateGameAsync(Arg.Any<GameSearchDocument>(), Arg.Any<CancellationToken>());
        await _priceHistoryRepository.DidNotReceive().GetByGameIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SearchFails_ShouldNotThrow()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGameWithId(1, "Test Game", 59.99m);
        var oldPrice = new Price(49.99m, "BRL");
        var newPrice = game.Price;
        var notification = new GamePriceChangedDomainEvent(game.Id, oldPrice, newPrice);

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

    [Fact]
    public async Task Handle_ShouldUpdateSearchIndex()
    {
        // Arrange
        var game = GameTestFactory.CreateValidGameWithId(1, "Test Game", 59.99m);
        var oldPrice = new Price(49.99m, "BRL");
        var newPrice = game.Price;
        var notification = new GamePriceChangedDomainEvent(game.Id, oldPrice, newPrice);

        _gameRepository.GetByIdAsync(game.Id, Arg.Any<CancellationToken>())
            .Returns(game);
        _priceHistoryRepository.GetByGameIdAsync(game.Id, Arg.Any<CancellationToken>())
            .Returns((GamePriceHistory?)null);
        _gameSearchRepository.UpdateGameAsync(Arg.Any<GameSearchDocument>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        _priceHistoryRepository.AddAsync(Arg.Any<GamePriceHistory>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await _gameSearchRepository.Received(1).UpdateGameAsync(
            Arg.Is<GameSearchDocument>(doc => doc.Id == game.Id && doc.Price == newPrice.Amount),
            Arg.Any<CancellationToken>()
        );
    }
}
