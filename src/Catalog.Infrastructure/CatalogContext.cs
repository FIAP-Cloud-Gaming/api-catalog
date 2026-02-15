using Catalog.Infrastructure.IntegrationEvents;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Catalog.Infrastructure;

/// <remark>
/// dotnet ef migrations add --startup-project ..\Catalog.API --context CatalogContext [migration-name]
/// </remark>
public class CatalogContext(DbContextOptions<CatalogContext> options, IMediator mediator)
    : DbContext(options), IUnitOfWork, INotificationContext
{
    public DbSet<Game> Games { get; set; } = null!;

    public DbSet<IntegrationEventLogEntry> IntegrationEventLogs { get; set; } = null!;

    private IDbContextTransaction? _currentTransaction;

    public IDbContextTransaction? GetCurrentTransaction() => _currentTransaction;

    public bool HasActiveTransaction => _currentTransaction != null;

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

    public async Task<IDbContextTransaction?> BeginTransactionAsync()
    {
        if (_currentTransaction != null) return null;

        _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        if (transaction != _currentTransaction)
            throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

        try
        {
            await SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            if (HasActiveTransaction)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public void RollbackTransaction()
    {
        try
        {
            _currentTransaction?.Rollback();
        }
        finally
        {
            if (HasActiveTransaction)
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }
    }
}
