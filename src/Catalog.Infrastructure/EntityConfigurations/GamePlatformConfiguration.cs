namespace Catalog.Infrastructure.EntityConfigurations;

public class GamePlatformConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        var converter = new ValueConverter<List<Platform>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<List<Platform>>(v, (JsonSerializerOptions?)null) ?? new List<Platform>());

        var comparer = new ValueComparer<List<Platform>>(
            (c1, c2) =>
                c1 != null && c2 != null && c1.SequenceEqual(c2),
            c =>
                c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c =>
                c == null ? new List<Platform>() : c.ToList()
        );

        builder.Property("_platforms")
            .HasColumnName("platforms")
            .HasColumnType("jsonb")
            .HasConversion(converter)
            .Metadata
            .SetValueComparer(comparer);

        builder.Property("_platforms")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
