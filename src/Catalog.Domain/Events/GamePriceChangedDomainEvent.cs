namespace Catalog.Domain.Events;

public record GamePriceChangedDomainEvent(
    int GameId,
    Price OldPrice,
    Price NewPrice) : INotification;
