namespace Catalog.Domain.ValueObjects;

public class GameTitle : ValueObject
{
    public string Value { get; private set; }

    private GameTitle()
    {
        Value = string.Empty;
    }

    public GameTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidGameTitleException(value);

        if (value.Length > 200)
            throw new InvalidGameTitleException(value);

        Value = value.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(GameTitle title) 
        => title.Value;
}
