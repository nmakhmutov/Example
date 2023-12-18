namespace Domain.Abstractions;

public interface IUnitOfWork : IDisposable
{
    Task<bool> SaveEntitiesAsync(CancellationToken ct = default);
}
