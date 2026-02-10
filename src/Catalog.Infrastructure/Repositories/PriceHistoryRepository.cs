using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

public class PriceHistoryRepository(IMongoDatabase database) 
    : IPriceHistoryRepository
{
    private readonly IMongoCollection<GamePriceHistory> _priceHistories = database.GetCollection<GamePriceHistory>("price_histories");

    public async Task<GamePriceHistory?> GetByGameIdAsync(int gameId, CancellationToken ct = default) 
        => await _priceHistories.Find(h => h.GameId == gameId)
            .FirstOrDefaultAsync(ct);

    public async Task AddAsync(GamePriceHistory history, CancellationToken ct = default) 
        => await _priceHistories.InsertOneAsync(history, cancellationToken: ct);

    public async Task UpdateAsync(GamePriceHistory history, CancellationToken ct = default) 
        => await _priceHistories.ReplaceOneAsync(
            h => h.Id == history.Id,
            history,
            cancellationToken: ct);
}
