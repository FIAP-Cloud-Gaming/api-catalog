using Catalog.UnitTests.Helpers;

namespace Catalog.UnitTests.Application.DomainEventHandlers;

public sealed class GameDeletedEventHandlerTests
{
    private readonly IGameSearchRepository _gameSearchRepository;
    private readonly ICatalogIntegrationEventService _integrationEventService;
    private readonly ILogger<GameDeletedEventHandler> _logger;
    private readonly GameDeletedEventHandler _handler;

    public GameDeletedEventHandlerTests()
    {
        _gameSearchRepository = Substitute.For<IGameSearchRepository>();
        _integrationEventService = Substitute.For<ICatalogIntegrationEventService>();
        _logger = Substitute.For<ILogger<GameDeletedEventHandler>>();
        _handler = new GameDeletedEventHandler(
            _gameSearchRepository,
            _integrationEventService,
            _logger
        );
    }

    [Fact]
    public async Task Handle_ShouldDeleteFromSearchIndex()
    {
        // Arrange
        var gameId = 1;
        var notification = new GameDeletedDomainEvent(gameId, "Test Game");

        _gameSearchRepository.DeleteGameAsync(gameId, Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await _gameSearchRepository.Received(1).DeleteGameAsync(gameId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DeleteFails_ShouldNotThrow()
    {
        // Arrange
        var gameId = 1;
        var notification = new GameDeletedDomainEvent(gameId, "Test Game");

        _gameSearchRepository.DeleteGameAsync(gameId, Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("Search service unavailable"));

        // Act
        var act = async () => await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Handle_ShouldCallDeleteGameAsync()
    {
        // Arrange
        var gameId = 42;
        var notification = new GameDeletedDomainEvent(gameId, "Test Game");

        _gameSearchRepository.DeleteGameAsync(gameId, Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        await _gameSearchRepository.Received(1).DeleteGameAsync(
            Arg.Is<int>(id => id == gameId),
            Arg.Any<CancellationToken>()
        );
    }
}
