namespace Example.Api.Application.Queries.GetUsers;

internal sealed record UserOverviewModel(long Id, string FirstName, string LastName, string Email, int Cards);
