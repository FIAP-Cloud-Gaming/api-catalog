namespace Catalog.API.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using CatalogContext dbContext =
            scope.ServiceProvider.GetRequiredService<CatalogContext>();

        dbContext.Database.Migrate();
    }
}
