using System;
using Microsoft.Extensions.DependencyInjection;

namespace Elwark.EventBus.RabbitMq
{
    public class ElwarkRabbitMqBuilder : IElwarkRabbitMqBuilder
    {
        public ElwarkRabbitMqBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IServiceCollection Services { get; }
    }
}