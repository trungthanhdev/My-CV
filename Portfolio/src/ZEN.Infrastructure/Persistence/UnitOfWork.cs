using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ZEN.Domain.Common.Abstractions;
using ZEN.Domain.Common.Primitives;
using MediatR;
using ZEN.Common.Domain.Events;
using CTCore.DynamicQuery.Core.Domain.Interfaces;

namespace ZEN.Infrastructure.Persistence;


public class UnitOfWork<TDbContext>(TDbContext dbContext, IPublisher publisher, ILogger<UnitOfWork<TDbContext>> logger)
    : IUnitOfWork
    where TDbContext : DbContext
{
    public string ContextName => typeof(TDbContext).Name;
    private IDbContextTransaction? _transaction;
    private readonly List<IDomainEvent> domainEvents = [];
    public async Task<int> SaveChangeAsync(CancellationToken cancellationToken = default)
    {
        Task[] tasks = [UpdateAuditableEntities(), UpdateSoftDeletableEntities()];
        await Task.WhenAll(tasks);
        try
        {
            GetDomainEvents();
            var ret = await dbContext.SaveChangesAsync(cancellationToken);
            _ = PublishDomainEvents().ConfigureAwait(false);
            return ret;
        }
        catch (DbUpdateException ex)
        {
            logger.LogError("Error saving changes ... {0}", ex.Message);
            dbContext.ChangeTracker.Clear();
            return -1;
        }
        catch (OperationCanceledException ex)
        {
            logger.LogError("Cancel by token ... {0}", ex.Message);
            dbContext.ChangeTracker.Clear();
            return -2;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return -3;
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction != null)
                await _transaction.CommitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            if (_transaction != null)
                await _transaction.RollbackAsync(cancellationToken);
            throw;
        }

    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
            await _transaction.RollbackAsync(cancellationToken);
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        dbContext.Dispose();
    }

    private Task UpdateAuditableEntities()
    {
        var entries = dbContext.ChangeTracker.Entries();

        List<EntityEntry> auditableEntries =
            entries is null ? [] :
            entries.Where(entry => entry.Entity is IAuditableEntity).ToList();

        foreach (var entry in auditableEntries)
        {
            if (entry.State == EntityState.Added)
            {
                var createdOnUtcProperty = entry.Entity.GetType().GetProperty("CreatedAt");
                createdOnUtcProperty?.SetValue(entry.Entity, DateTimeOffset.UtcNow, null);
            }

            if (entry.State == EntityState.Modified || entry.State == EntityState.Unchanged)
            {
                var modifiedOnUtcProperty = entry.Entity.GetType().GetProperty("ModifiedOn");
                modifiedOnUtcProperty?.SetValue(entry.Entity, DateTimeOffset.UtcNow, null);
            }
        }
        return Task.CompletedTask;
    }

    private Task UpdateSoftDeletableEntities()
    {
        var entries = dbContext.ChangeTracker.Entries();
        List<EntityEntry> deletableEntries = entries is null ? [] :
            entries.Where(entry => entry.Entity is ISoftDeletableEntity).ToList();

        foreach (var entry in deletableEntries)
        {
            if (entry.State != EntityState.Deleted)
            {
                continue;
            }

            var deletedOn = entry.Entity.GetType().GetProperty("DeletedOn");
            deletedOn?.SetValue(entry.Entity, DateTimeOffset.UtcNow, null);
        }
        return Task.CompletedTask;
    }
    private void GetDomainEvents()
    {
        var entities = dbContext.ChangeTracker.Entries<AggregateRoot>()
                    .Select(e => e.Entity)
                    .Where(e => e.DomainEvents.Count != 0).ToList();

        foreach (var entity in entities)
        {
            domainEvents.AddRange(entity.DomainEvents);

            entity.ClearDomainEvents();
        }
    }
    private Task PublishDomainEvents()
    {
        foreach (var de in domainEvents)
        {
            _ = publisher.Publish(de).ConfigureAwait(false);
        }

        return Task.CompletedTask;
    }

    [Obsolete]
    public void DisableTransaction()
    {
        dbContext.Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
    }

}