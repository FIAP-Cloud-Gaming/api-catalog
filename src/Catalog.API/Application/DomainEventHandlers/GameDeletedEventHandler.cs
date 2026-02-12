namespace Catalog.API.Application.DomainEventHandlers;

public class GameDeletedEventHandler(
    IGameSearchRepository searchRepository,
    IEventBus eventBus,
    ILogger<GameDeletedEventHandler> logger)
    : INotificationHandler<GameDeletedDomainEvent>
{
    public async Task Handle(GameDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Handling GameDeletedDomainEvent for game {GameId}", notification.GameId);

            await searchRepository.DeleteGameAsync(notification.GameId, cancellationToken);

            await eventBus.PublishAsync(new GameDeletedIntegrationEvent(notification.GameId, notification.Title));
            logger.LogInformation("Game {GameId} deleted from Elasticsearch and integration event published", notification.GameId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting game {GameId} from search index", notification.GameId);
        }
    }
}
