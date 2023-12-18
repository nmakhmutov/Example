using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Example.Infrastructure;

internal sealed class UserDbContextDesignFactory : IDesignTimeDbContextFactory<UserDbContext>
{
    public UserDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<UserDbContext>()
            .UseNpgsql("Host=_;Database=_;Username=_;Password=_");

        return new UserDbContext(builder.Options, new NoMediator());
    }

    private sealed class NoMediator : IMediator
    {
        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> _, CancellationToken __) =>
            default!;

        public IAsyncEnumerable<object?> CreateStream(object _, CancellationToken __) =>
            default!;

        public Task Publish<TNotification>(TNotification _, CancellationToken __)
            where TNotification : INotification =>
            Task.CompletedTask;

        public Task Publish(object _, CancellationToken __) =>
            Task.CompletedTask;

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> _, CancellationToken __) =>
            Task.FromResult<TResponse>(default!);

        public Task Send<TRequest>(TRequest _, CancellationToken __)
            where TRequest : IRequest =>
            Task.CompletedTask;

        public Task<object?> Send(object _, CancellationToken __) =>
            Task.FromResult(default(object));
    }
}
