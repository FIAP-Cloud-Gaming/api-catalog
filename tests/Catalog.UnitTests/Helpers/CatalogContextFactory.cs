namespace Catalog.UnitTests.Helpers;

internal static class CatalogContextFactory
{
    public static CatalogContext Create(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<CatalogContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var mediator = Substitute.For<IMediator>();
        return new CatalogContext(options, mediator);
    }
}
