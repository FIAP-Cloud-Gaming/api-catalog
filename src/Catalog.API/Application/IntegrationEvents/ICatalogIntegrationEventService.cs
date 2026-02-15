namespace Catalog.API.Application.IntegrationEvents;

public interface ICatalogIntegrationEventService
{
    Task PublishEventsThroughEventBusAsync(Guid transactionId);
    Task AddAndSaveEventAsync(object @event);
}
