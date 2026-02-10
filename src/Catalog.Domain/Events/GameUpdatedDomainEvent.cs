namespace Catalog.Domain.Events;

public record GameUpdatedDomainEvent(int GameId, string Title) : INotification;
