using Example.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Example.Infrastructure.ValueConverters;

internal sealed class UserIdConverter : ValueConverter<UserId, long>
{
    public UserIdConverter()
        : base(x => x, x => new UserId(x))
    {
    }
}
