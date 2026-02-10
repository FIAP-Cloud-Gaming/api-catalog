using Microsoft.EntityFrameworkCore.Design;

namespace Catalog.Infrastructure;

public class CatalogContextDesignTimeFactory : IDesignTimeDbContextFactory<CatalogContext>
{
    public CatalogContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CatalogContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=CatalogDB;Username=postgres;Password=postgres",
            npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(CatalogContext).Assembly.FullName);
            });

        return new CatalogContext(optionsBuilder.Options, null!);
    }
}
