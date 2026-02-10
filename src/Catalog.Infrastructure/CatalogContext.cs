namespace Catalog.Infrastructure;

/// <remark>
/// dotnet ef migrations add --startup-project ..\Catalog.API --context CatalogContext [migration-name]
/// </remark>
public class CatalogContext(DbContextOptions<CatalogContext> options, IMediator mediator) 
    : DbContext(options), IUnitOfWork, INotificationContext
{
    public DbSet<Game> Games { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("catalog");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogContext).Assembly);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await mediator.DispatchNotificationsAsync(this, cancellationToken);

        await base.SaveChangesAsync(cancellationToken);

        return true;
    }

    public IEnumerable<EntityWithNotification> GetEntitiesWithNotifications()
        => [.. ChangeTracker
            .Entries<EntityWithNotification>()
            .Where(entry => entry.Entity.Notifications.Count != 0)
            .Select(entry => entry.Entity)];
}
