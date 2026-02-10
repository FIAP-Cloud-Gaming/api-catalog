namespace Catalog.Domain.AggregatesModel.PriceHistoryAggregate;

public class GamePriceHistory : EntityWithNotification, IAggregateRoot
{
    public int GameId { get; private set; }

    private readonly List<PriceChangedEvent> _events = [];
    public IReadOnlyCollection<PriceChangedEvent> Events => _events.AsReadOnly();

    protected GamePriceHistory() { }

    public GamePriceHistory(int gameId)
    {
        GameId = gameId;
    }

    public void RecordPriceChange(
        decimal oldPrice,
        decimal newPrice,
        string currency,
        string reason,
        string changedBy)
    {
        var priceEvent = new PriceChangedEvent(
            GameId,
            oldPrice,
            newPrice,
            currency,
            reason,
            changedBy);

        _events.Add(priceEvent);

        AddNotification(new PriceHistoryRecordedDomainEvent(GameId, newPrice));
    }

    public decimal GetCurrentPrice()
    {
        if (_events.Count == 0)
            return 0;

        return _events.Last().NewPrice;
    }

    public IEnumerable<PriceChangedEvent> GetHistory()
    {
        return _events.OrderByDescending(e => e.ChangedAt);
    }

    public IEnumerable<PriceChangedEvent> GetHistoryInPeriod(DateTime startDate, DateTime endDate)
    {
        return _events
            .Where(e => e.ChangedAt >= startDate && e.ChangedAt <= endDate)
            .OrderByDescending(e => e.ChangedAt);
    }
}
