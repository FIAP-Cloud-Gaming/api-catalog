namespace Catalog.Domain.Events;

public record GameCreatedDomainEvent(int GameId, string Title) : INotification;
