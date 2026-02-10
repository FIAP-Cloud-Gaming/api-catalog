namespace Catalog.Infrastructure.EntityConfigurations;

public class GameGenreConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        var converter = new ValueConverter<List<Genre>, string>(
           v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
           v => JsonSerializer.Deserialize<List<Genre>>(v, (JsonSerializerOptions?)null)
                ?? new List<Genre>()
       );

        var comparer = new ValueComparer<List<Genre>>(
            (c1, c2) =>
                c1 != null && c2 != null && c1.SequenceEqual(c2),
            c =>
                c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c =>
                c == null ? new List<Genre>() : c.ToList()
        );

        builder.Property("_genres")
            .HasColumnName("genres")
            .HasColumnType("jsonb")
            .HasConversion(converter)
            .Metadata.SetValueComparer(comparer);

        builder.Property("_genres")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
