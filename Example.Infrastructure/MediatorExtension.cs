using Domain.Abstractions;
using MediatR;

namespace Example.Infrastructure;

internal static class MediatorExtension
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, UserDbContext ctx)
    {
        foreach (var entry in ctx.ChangeTracker.Entries<IAggregateRoot>())
        {
            foreach (var evt in entry.Entity.GetDomainEvents())
                await mediator.Publish(evt);

            entry.Entity.ClearDomainEvents();
        }
    }
}