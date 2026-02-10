namespace Catalog.API.Application.DomainEventHandlers;

public class GameDeletedEventHandler(
    IGameSearchRepository searchRepository,
    ILogger<GameDeletedEventHandler> logger)
    : INotificationHandler<GameDeletedDomainEvent>
{
    public async Task Handle(GameDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Handling GameDeletedDomainEvent for game {GameId}", notification.GameId);

            await searchRepository.DeleteGameAsync(notification.GameId, cancellationToken);

            logger.LogInformation("Game {GameId} deleted from Elasticsearch successfully", notification.GameId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting game {GameId} from search index", notification.GameId);
        }
    }
}
