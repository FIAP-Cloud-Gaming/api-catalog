namespace Catalog.Domain.AggregatesModel.PriceHistoryAggregate;

public class PriceChangedEvent : Entity
{
    public int GameId { get; private set; }
    public decimal OldPrice { get; private set; }
    public decimal NewPrice { get; private set; }
    public string Currency { get; private set; }
    public string Reason { get; private set; }
    public string ChangedBy { get; private set; }
    public DateTime ChangedAt { get; private set; }

    protected PriceChangedEvent()
    {
        Currency = string.Empty;
        Reason = string.Empty;
        ChangedBy = string.Empty;
    }

    public PriceChangedEvent(
        int gameId,
        decimal oldPrice,
        decimal newPrice,
        string currency,
        string reason,
        string changedBy)
    {
        GameId = gameId;
        OldPrice = oldPrice;
        NewPrice = newPrice;
        Currency = currency ?? "BRL";
        Reason = reason ?? string.Empty;
        ChangedBy = changedBy ?? "system";
        ChangedAt = DateTime.UtcNow;
    }
}
