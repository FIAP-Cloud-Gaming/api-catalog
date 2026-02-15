namespace Catalog.API.Application.DomainEventHandlers;

public class GameUnpublishedEventHandler(
    IGameRepository gameRepository,
    IGameSearchRepository searchRepository,
    ICatalogIntegrationEventService integrationEventService,
    ILogger<GameUnpublishedEventHandler> logger)
    : INotificationHandler<GameUnpublishedDomainEvent>
{
    public async Task Handle(GameUnpublishedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Handling GameUnpublishedDomainEvent for game {GameId}", notification.GameId);

            var game = await gameRepository.GetByIdAsync(notification.GameId, cancellationToken);
            if (game is null)
            {
                logger.LogWarning("Game {GameId} not found in repository", notification.GameId);
                return;
            }

            var searchDoc = GameSearchDocument.FromGame(game);
            await searchRepository.UpdateGameAsync(searchDoc, cancellationToken);

            await integrationEventService.AddAndSaveEventAsync(new GameUnpublishedIntegrationEvent(game.Id, game.Title.Value));
            logger.LogInformation("Game {GameId} unpublished and integration event published", notification.GameId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating unpublished status for game {GameId} in search index", notification.GameId);
        }
    }
}
