namespace Catalog.Infrastructure.EntityConfigurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("games");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.OwnsOne(g => g.Title, title =>
        {
            title.Property(t => t.Value)
                .HasColumnName("title")
                .HasMaxLength(200)
                .IsRequired();
        });

        builder.Property(g => g.Description)
            .HasColumnName("description")
            .HasMaxLength(2000);

        builder.OwnsOne(g => g.Price, price =>
        {
            price.Property(p => p.Amount)
                .HasColumnName("price")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            price.Property(p => p.Currency)
                .HasColumnName("currency")
                .HasMaxLength(3)
                .HasDefaultValue("USD")
                .IsRequired();
        });

        builder.Property(g => g.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(g => g.ReleaseDate)
            .HasColumnName("release_date");

        builder.Property(g => g.Publisher)
            .HasColumnName("publisher")
            .HasMaxLength(200);

        builder.Property(g => g.Developer)
            .HasColumnName("developer")
            .HasMaxLength(200);

        builder.Property(g => g.CoverImageUrl)
            .HasColumnName("cover_image_url")
            .HasMaxLength(500);

        builder.Property(g => g.AverageRating)
            .HasColumnName("average_rating")
            .HasColumnType("decimal(3,2)")
            .HasDefaultValue(0);

        builder.Property(g => g.TotalReviews)
            .HasColumnName("total_reviews")
            .HasDefaultValue(0);

        builder.Property(g => g.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(g => g.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasIndex(g => g.Status)
            .HasDatabaseName("ix_games_status");

        builder.HasIndex(g => g.CreatedAt)
            .HasDatabaseName("ix_games_created_at");

        builder.Ignore(g => g.Notifications);
    }
}
