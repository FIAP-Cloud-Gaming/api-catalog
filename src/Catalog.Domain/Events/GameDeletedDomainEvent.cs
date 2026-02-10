namespace Catalog.Domain.Events;

public record GameDeletedDomainEvent(int GameId, string Title) : INotification;
