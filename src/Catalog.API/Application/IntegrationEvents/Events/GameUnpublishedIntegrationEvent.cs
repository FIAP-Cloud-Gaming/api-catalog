namespace Catalog.API.Application.IntegrationEvents.Events;

public record GameUnpublishedIntegrationEvent(int GameId, string Title) : IntegrationEvent;
