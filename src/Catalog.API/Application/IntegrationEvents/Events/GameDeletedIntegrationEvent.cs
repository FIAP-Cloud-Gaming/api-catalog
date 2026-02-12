namespace Catalog.API.Application.IntegrationEvents.Events;

public record GameDeletedIntegrationEvent(int GameId, string Title) : IntegrationEvent;
