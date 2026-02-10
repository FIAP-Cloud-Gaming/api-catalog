namespace Catalog.Domain.AggregatesModel.GameAggregate;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Platform
{
    PC = 1,
    PlayStation5 = 2,
    PlayStation4 = 3,
    XboxSeriesX = 4,
    XboxSeriesS = 5,
    XboxOne = 6,
    NintendoSwitch = 7,
    iOS = 8,
    Android = 9,
    MacOS = 10,
    Linux = 11,
    Web = 12
}
