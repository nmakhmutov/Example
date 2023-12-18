using System.Data;
using Domain.Abstractions;
using Example.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Example.Infrastructure;

public class UserDbContext : DbContext,
    IUnitOfWork
{
    private readonly IMediator _mediator;

    private IDbContextTransaction? _currentTransaction;

    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options) =>
        _mediator = default!;

    public UserDbContext(DbContextOptions<UserDbContext> options, IMediator mediator)
        : base(options) =>
        _mediator = mediator;

    public DbSet<User> Users =>
        Set<User>();

    public bool HasActiveTransaction =>
        _currentTransaction is not null;

    public async Task<bool> SaveEntitiesAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<ICreated>())
            entry.Entity.SetAsUpdated(now);

        await base.SaveChangesAsync(ct);

        await _mediator.DispatchDomainEventsAsync(this);

        return true;
    }

    public IDbContextTransaction GetCurrentTransaction() =>
        _currentTransaction ?? throw new InvalidOperationException("Transaction is not initialized");

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction is null)
            return _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);

        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
        if (_currentTransaction != transaction)
            throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

        try
        {
            await SaveChangesAsync();

            await _currentTransaction.CommitAsync();
        }
        catch
        {
            await _currentTransaction.RollbackAsync();
            throw;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);
}
