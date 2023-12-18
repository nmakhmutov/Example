using Example.Domain;
using Example.Domain.Exceptions;
using Example.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Example.Api.Application.Queries.GetUser;

internal sealed record GetUserQuery(UserId Id) : IRequest<UserDetailModel>;

internal sealed record GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDetailModel>
{
    private readonly UserDbContext _dbContext;

    public GetUserQueryHandler(UserDbContext dbContext) =>
        _dbContext = dbContext;

    public async Task<UserDetailModel> Handle(GetUserQuery request, CancellationToken ct) =>
        await _dbContext.Users
            .Where(x => x.Id == request.Id)
            .Select(x => new UserDetailModel(
                    x.Id,
                    x.Name.FirstName,
                    x.Name.LastName,
                    x.Email,
                    x.Cards.Select(c => new UserDetailModel.CardModel(c.Name, c.Number))
                )
            )
            .FirstOrDefaultAsync(ct) ?? throw new UserException($"User {request.Id} not found");
}