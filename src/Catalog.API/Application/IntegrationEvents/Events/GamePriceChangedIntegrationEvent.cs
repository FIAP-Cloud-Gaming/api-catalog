namespace Catalog.API.Application.IntegrationEvents.Events;

public record GamePriceChangedIntegrationEvent(int GameId, decimal OldPrice, decimal NewPrice, string Currency) : IntegrationEvent;
