namespace Catalog.API.Application.IntegrationEvents.Events;

public record GameCreatedIntegrationEvent(int GameId, string Title, decimal Price, string Currency) : IntegrationEvent;
