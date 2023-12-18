namespace Domain.Abstractions;

public interface IRepository<TRoot, in TId>
    where TRoot : IAggregateRoot<TId>
    where TId : struct
{
    public Task<TRoot?> GetAsync(TId id, CancellationToken ct = default);

    public Task<TRoot> CreateAsync(TRoot entity, CancellationToken ct = default);

    public Task<TRoot> UpdateAsync(TRoot entity, CancellationToken ct = default);

    public Task DeleteAsync(TId id, CancellationToken ct = default);
}
