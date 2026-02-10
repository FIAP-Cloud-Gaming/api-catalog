namespace Catalog.Domain.AggregatesModel.PriceHistoryAggregate;

public interface IPriceHistoryRepository
{
    Task<GamePriceHistory?> GetByGameIdAsync(int gameId, CancellationToken cancellationToken = default);
    Task AddAsync(GamePriceHistory history, CancellationToken cancellationToken = default);
    Task UpdateAsync(GamePriceHistory history, CancellationToken cancellationToken = default);
}
