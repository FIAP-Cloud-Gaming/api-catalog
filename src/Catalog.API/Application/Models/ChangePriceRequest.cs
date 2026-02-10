namespace Catalog.API.Application.Models;

public sealed record ChangePriceRequest
{
    public required decimal Amount { get; init; }
    public string Currency { get; init; } = "BRL";
}
