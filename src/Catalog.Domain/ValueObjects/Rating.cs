namespace Catalog.Domain.ValueObjects;

public class Rating : ValueObject
{
    public int Value { get; private set; }

    private Rating() { }

    public Rating(int value)
    {
        if (value < 1 || value > 5)
            throw new InvalidRatingException(value);

        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => $"{Value}/5";

    public static implicit operator int(Rating rating) => rating.Value;
}
