namespace Catalog.API.Application.IntegrationEvents.Events;

public record GamePublishedIntegrationEvent(int GameId, string Title) : IntegrationEvent;
