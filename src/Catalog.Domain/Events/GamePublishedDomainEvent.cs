namespace Catalog.Domain.Events;

public record GamePublishedDomainEvent(int GameId, string Title) : INotification;
