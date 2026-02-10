namespace Catalog.Domain.Events;

public record GameUnpublishedDomainEvent(int GameId, string Title) : INotification;
