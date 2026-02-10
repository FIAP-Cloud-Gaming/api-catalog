namespace Catalog.Domain.Events;

public record PriceHistoryRecordedDomainEvent(int GameId, decimal NewPrice) : INotification;
