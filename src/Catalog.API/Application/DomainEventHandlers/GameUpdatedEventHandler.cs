namespace Catalog.API.Application.DomainEventHandlers;

public class GameUpdatedEventHandler(
    IGameRepository gameRepository,
    IGameSearchRepository searchRepository,
    IEventBus eventBus,
    ILogger<GameUpdatedEventHandler> logger)
    : INotificationHandler<GameUpdatedDomainEvent>
{
    public async Task Handle(GameUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Handling GameUpdatedDomainEvent for game {GameId}", notification.GameId);

            var game = await gameRepository.GetByIdAsync(notification.GameId, cancellationToken);
            if (game is null)
            {
                logger.LogWarning("Game {GameId} not found in repository", notification.GameId);
                return;
            }

            var searchDoc = GameSearchDocument.FromGame(game);
            await searchRepository.UpdateGameAsync(searchDoc, cancellationToken);

            await eventBus.PublishAsync(new GameUpdatedIntegrationEvent(game.Id, game.Title.Value));
            logger.LogInformation("Game {GameId} updated in Elasticsearch and integration event published", notification.GameId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating game {GameId} in search index", notification.GameId);
        }
    }
}
