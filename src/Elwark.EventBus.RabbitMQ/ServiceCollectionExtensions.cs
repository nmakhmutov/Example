using Microsoft.Extensions.DependencyInjection;
using RawRabbit;
using RawRabbit.DependencyInjection.ServiceCollection;
using RawRabbit.Enrichers.MessageContext;
using RawRabbit.Instantiation;

namespace Elwark.EventBus.RabbitMq
{
    public static class ServiceCollectionExtensions
    {
        public static IElwarkRabbitMqBuilder AddElwarkRabbitMq(this IServiceCollection services,
            ElwarkRabbitConfiguration configuration)
        {
            var builder = new ElwarkRabbitMqBuilder(services);

            builder.Services.AddRawRabbit(new RawRabbitOptions
            {
                ClientConfiguration = configuration,
                Plugins = plugins => plugins
                    .UseRetryLater()
                    .UseContextForwarding()
                    .UseMessageContext<ElwarkMessageContext>()
            });

            builder.Services.AddSingleton(configuration);
            builder.Services.AddSingleton<IIntegrationEventPublisher, RabbitMqEventPublisher>();

            return builder;
        }

        public static IElwarkRabbitMqBuilder AddEventHandler<TEvent, TEventHandler>(this IElwarkRabbitMqBuilder builder)
            where TEvent : IntegrationEvent
            where TEventHandler : class, IIntegrationEventHandler<TEvent>
        {
            builder.Services.AddTransient<IIntegrationEventHandler<TEvent>, TEventHandler>();
            builder.Services.AddHostedService<RabbitMqBackgroundEventHandler<TEvent>>();

            return builder;
        }
    }
}