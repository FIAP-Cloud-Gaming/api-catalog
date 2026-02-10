namespace Catalog.API.Application.DomainEventHandlers;

public class GameCreatedEventHandler(
    IGameRepository gameRepository,
    IGameSearchRepository searchRepository,
    IPriceHistoryRepository priceHistoryRepository,
    ILogger<GameCreatedEventHandler> logger)
    : INotificationHandler<GameCreatedDomainEvent>
{
    public async Task Handle(GameCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Handling GameCreatedDomainEvent for game {GameId}", notification.GameId);

            var game = await gameRepository.GetByIdAsync(notification.GameId, cancellationToken);
            if (game is null)
            {
                logger.LogWarning("Game {GameId} not found in repository", notification.GameId);
                return;
            }

            var searchDoc = GameSearchDocument.FromGame(game);
            await searchRepository.IndexGameAsync(searchDoc, cancellationToken);
            logger.LogInformation("Game {GameId} indexed in Elasticsearch", notification.GameId);

            var history = new GamePriceHistory(game.Id);
            history.RecordPriceChange(
                oldPrice: 0,
                newPrice: game.Price.Amount,
                currency: game.Price.Currency,
                reason: "Initial price",
                changedBy: "System");

            await priceHistoryRepository.AddAsync(history, cancellationToken);
            logger.LogInformation("Price history created for game {GameId}", notification.GameId);

            logger.LogInformation("Game {GameId} synchronized to search and price history successfully", notification.GameId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error synchronizing game {GameId} after creation", notification.GameId);
        }
    }
}
