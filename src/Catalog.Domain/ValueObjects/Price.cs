namespace Catalog.Domain.ValueObjects;

public class Price : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    private Price()
    {
        Currency = "BRL"; 
    }

    public Price(decimal amount, string currency = "BRL")
    {
        if (amount < 0)
            throw new InvalidPriceException(amount);

        Amount = amount;
        Currency = currency ?? "BRL";
    }

    public static Price Free() => new(0);

    public Price ApplyDiscount(decimal discountPercentage)
    {
        if (discountPercentage < 0 || discountPercentage > 100)
            throw new CatalogDomainException("Discount percentage must be between 0 and 100.");

        var discountedAmount = Math.Round(Amount - (Amount * discountPercentage / 100), 2);
        return new Price(discountedAmount, Currency);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:F2} {Currency}";
}
