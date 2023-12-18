using Domain.Abstractions;
using Example.Domain;
using Example.Domain.Entities;
using Example.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Example.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly UserDbContext _dbContext;

    public UserRepository(UserDbContext dbContext) =>
        _dbContext = dbContext;

    public IUnitOfWork UnitOfWork =>
        _dbContext;

    public Task<User?> GetAsync(UserId id, CancellationToken ct) =>
        _dbContext.Users
            .Include(x => x.Cards)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<User?> GetAsync(string email, CancellationToken ct = default) =>
        _dbContext.Users
            .Include(x => x.Cards)
            .FirstOrDefaultAsync(x => x.Email == email, ct);

    public async Task<User> CreateAsync(User entity, CancellationToken ct) =>
        (await _dbContext.Users.AddAsync(entity, ct)).Entity;

    public Task<User> UpdateAsync(User entity, CancellationToken ct = default) =>
        Task.FromResult(_dbContext.Users.Update(entity).Entity);

    public async Task DeleteAsync(UserId id, CancellationToken ct = default)
    {
        var entity = await GetAsync(id, ct);
        if (entity is null)
            return;

        _dbContext.Users.Remove(entity);

        // .NET 7
        // _dbContext.Users
        //     .Where(x => x.Id == id)
        //     .ExecuteDeleteAsync(ct);
    }
}
