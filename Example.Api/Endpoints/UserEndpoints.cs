using Example.Api.Application.Commands.AddBackCard;
using Example.Api.Application.Commands.CreateUser;
using Example.Api.Application.Queries.GetUser;
using Example.Api.Application.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Example.Api.Endpoints;

internal static class UserEndpoints
{
    public static void UserUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var builder = endpoints.MapGroup("users")
            .WithTags("users")
            .WithOpenApi();

        builder.MapGet("/", GetUsersAsync);
        builder.MapGet("/{id:long}", GetUserAsync);
        builder.MapPost("/", CreateUserAsync);
        builder.MapPost("/{id:long}/cards", AddCardAsync);
    }

    private static IAsyncEnumerable<UserOverviewModel> GetUsersAsync(ISender sender, CancellationToken ct)
    {
        return sender.CreateStream(new GetUsersQuery(), ct);
    }

    private static async Task<Ok<UserDetailModel>> GetUserAsync(long id, ISender sender, CancellationToken ct)
    {
        var user = await sender.Send(new GetUserQuery(id), ct);

        return TypedResults.Ok(user);
    }

    private static async Task<NoContent> AddCardAsync(long id, AddCardRequest request, ISender sender,
        CancellationToken ct)
    {
        var command = new AddBackCardCommand(id, request.CardNumber, request.CardName, request.Expiry);
        await sender.Send(command, ct);

        return TypedResults.NoContent();
    }

    private static async Task<Ok<UserDetailModel>> CreateUserAsync(CreateUserCommand command, ISender sender,
        CancellationToken ct)
    {
        var id = await sender.Send(command, ct);
        var user = await sender.Send(new GetUserQuery(id), ct);

        return TypedResults.Ok(user);
    }

    private sealed record AddCardRequest(string CardNumber, string CardName, DateOnly Expiry);
}
