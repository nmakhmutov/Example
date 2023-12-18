using Domain.Abstractions;
using Example.Domain.Entities;

namespace Example.Domain.Repositories;

public interface IUserRepository : IRepository<User, UserId>
{
    IUnitOfWork UnitOfWork { get; }

    public Task<User?> GetAsync(string email, CancellationToken ct = default);
}
