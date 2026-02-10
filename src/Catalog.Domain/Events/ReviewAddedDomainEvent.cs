namespace Catalog.Domain.Events;

public record ReviewAddedDomainEvent(
    int GameId,
    string UserId,
    int Rating) : INotification;
