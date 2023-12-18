namespace Example.Api.Application.Queries.GetUser;

internal sealed record UserDetailModel(
    long Id,
    string FirstName,
    string LastName,
    string Email,
    IEnumerable<UserDetailModel.CardModel> Cards
)
{
    internal sealed record CardModel(string Name, string Number);
}
