namespace Catalog.API.Application.DomainEventHandlers;

public class GamePublishedEventHandler(
    IGameRepository gameRepository,
    IGameSearchRepository searchRepository,
    IEventBus eventBus,
    ILogger<GamePublishedEventHandler> logger)
    : INotificationHandler<GamePublishedDomainEvent>
{
    public async Task Handle(GamePublishedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Handling GamePublishedDomainEvent for game {GameId}", notification.GameId);

            var game = await gameRepository.GetByIdAsync(notification.GameId, cancellationToken);
            if (game is null)
            {
                logger.LogWarning("Game {GameId} not found in repository", notification.GameId);
                return;
            }

            var searchDoc = GameSearchDocument.FromGame(game);
            await searchRepository.UpdateGameAsync(searchDoc, cancellationToken);

            await eventBus.PublishAsync(new GamePublishedIntegrationEvent(game.Id, game.Title.Value));
            logger.LogInformation("Game {GameId} published and integration event published", notification.GameId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating published status for game {GameId} in search index", notification.GameId);
        }
    }
}
