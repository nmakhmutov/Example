using Example.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Example.Api.Application.Queries.GetUsers;

internal sealed record GetUsersQuery : IStreamRequest<UserOverviewModel>;

internal sealed class GetUsersQueryHandler : IStreamRequestHandler<GetUsersQuery, UserOverviewModel>
{
    private readonly UserDbContext _dbContext;

    public GetUsersQueryHandler(UserDbContext dbContext) =>
        _dbContext = dbContext;

    public IAsyncEnumerable<UserOverviewModel> Handle(GetUsersQuery request, CancellationToken ct) =>
        _dbContext.Users
            .Select(x => new UserOverviewModel(x.Id, x.Name.FirstName, x.Name.LastName, x.Email, x.Cards.Count))
            .AsAsyncEnumerable();
}