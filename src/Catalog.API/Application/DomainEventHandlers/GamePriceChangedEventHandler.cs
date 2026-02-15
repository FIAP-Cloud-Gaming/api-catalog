namespace Catalog.API.Application.DomainEventHandlers;

public class GamePriceChangedEventHandler(
    IGameRepository gameRepository,
    IGameSearchRepository searchRepository,
    IPriceHistoryRepository priceHistoryRepository,
    ICatalogIntegrationEventService integrationEventService,
    ILogger<GamePriceChangedEventHandler> logger)
    : INotificationHandler<GamePriceChangedDomainEvent>
{
    public async Task Handle(GamePriceChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(
                "Handling GamePriceChangedDomainEvent for game {GameId}: {OldPrice} -> {NewPrice}",
                notification.GameId,
                notification.OldPrice.Amount,
                notification.NewPrice.Amount);

            var game = await gameRepository.GetByIdAsync(notification.GameId, cancellationToken);
            if (game is null)
            {
                logger.LogWarning("Game {GameId} not found in repository", notification.GameId);
                return;
            }

            var searchDoc = GameSearchDocument.FromGame(game);
            await searchRepository.UpdateGameAsync(searchDoc, cancellationToken);
            logger.LogInformation("Game {GameId} price updated in Elasticsearch", notification.GameId);

            var history = await priceHistoryRepository.GetByGameIdAsync(notification.GameId, cancellationToken);

            if (history is null)
            {
                history = new GamePriceHistory(notification.GameId);
                history.RecordPriceChange(
                    oldPrice: notification.OldPrice.Amount,
                    newPrice: notification.NewPrice.Amount,
                    currency: notification.NewPrice.Currency,
                    reason: "Price changed",
                    changedBy: "System");

                await priceHistoryRepository.AddAsync(history, cancellationToken);
            }
            else
            {
                history.RecordPriceChange(
                    oldPrice: notification.OldPrice.Amount,
                    newPrice: notification.NewPrice.Amount,
                    currency: notification.NewPrice.Currency,
                    reason: "Price changed",
                    changedBy: "System");

                await priceHistoryRepository.UpdateAsync(history, cancellationToken);
            }

            logger.LogInformation("Price history updated for game {GameId}", notification.GameId);

            await integrationEventService.AddAndSaveEventAsync(new GamePriceChangedIntegrationEvent(
                notification.GameId, notification.OldPrice.Amount, notification.NewPrice.Amount, notification.NewPrice.Currency));
            logger.LogInformation("Game {GameId} price change synchronized and integration event published", notification.GameId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error synchronizing price change for game {GameId}", notification.GameId);
        }
    }
}
