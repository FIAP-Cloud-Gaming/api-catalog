namespace Catalog.API.Application.IntegrationEvents.Events;

public record GameUpdatedIntegrationEvent(int GameId, string Title) : IntegrationEvent;
