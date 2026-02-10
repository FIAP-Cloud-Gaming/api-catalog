namespace Catalog.Domain.AggregatesModel.GameAggregate;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GameStatus
{
    Draft = 1,
    Published = 2,
    Unpublished = 3,
    Archived = 4
}
