using Microsoft.Extensions.DependencyInjection;

namespace Elwark.EventBus.RabbitMq
{
    public interface IElwarkRabbitMqBuilder
    {
        IServiceCollection Services { get; }
    }
}