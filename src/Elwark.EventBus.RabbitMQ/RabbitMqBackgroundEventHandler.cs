using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RawRabbit;
using RawRabbit.Common;
using RawRabbit.Configuration.Exchange;
using RawRabbit.Enrichers.MessageContext.Context;
using RawRabbit.Enrichers.MessageContext.Subscribe;
using RawRabbit.Exceptions;
using RawRabbit.Operations.Subscribe.Context;
using RawRabbit.Pipe;

namespace Elwark.EventBus.RabbitMq
{
    public class RabbitMqBackgroundEventHandler<TEvent> : BackgroundService
        where TEvent : IntegrationEvent
    {
        private readonly IBusClient _busClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RabbitMqBackgroundEventHandler<TEvent>> _logger;
        private readonly ElwarkRabbitConfiguration _configuration;

        public RabbitMqBackgroundEventHandler(IBusClient busClient, IServiceProvider serviceProvider,
            ILogger<RabbitMqBackgroundEventHandler<TEvent>> logger, ElwarkRabbitConfiguration configuration)
        {
            _busClient = busClient ?? throw new ArgumentNullException(nameof(busClient));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Subscribing to event {event}", typeof(TEvent).Name);

            await _busClient.SubscribeAsync<TEvent, ElwarkMessageContext>(async (evt, context) =>
                {
                    _logger.LogInformation("Received message {id}. Retry {count}", evt.Id,
                        context.RetryInformation.NumberOfRetries);

                    if (context.RetryInformation.NumberOfRetries >= _configuration.RetryCount)
                    {
                        _logger.LogCritical("Event {evt} with body {@body} rejected after {count} handling attempt",
                            evt.GetType().Name, evt, context.RetryInformation.NumberOfRetries);
                        return new Reject(false);
                    }

                    using var services = _serviceProvider.CreateScope();
                    var handlers = services.ServiceProvider.GetServices<IIntegrationEventHandler<TEvent>>();
                    foreach (var handler in handlers)
                    {
                        stoppingToken.ThrowIfCancellationRequested();

                        try
                        {
                            await handler.HandleAsync(evt, stoppingToken);
                        }
                        catch
                        {
                            _logger.LogWarning("Error has occurred when handled event {evt} by handler {handler}",
                                evt.GetType().Name, handler.GetType().Name);
                            return new Retry(TimeSpan.FromSeconds(10 * context.RetryInformation.NumberOfRetries));
                        }
                    }

                    return new Ack();
                },
                ctx => Configuration(ctx, typeof(TEvent).Name),
                stoppingToken);
        }

        private void Configuration(ISubscribeContext ctx, string routingKey) =>
            ctx.UseMessageContext(context => new ElwarkMessageContext
                {
                    RetryInformation = context.GetRetryInformation(),
                    GlobalRequestId = Guid.NewGuid()
                })
                .UseConsumerConcurrency(_configuration.PrefetchCount)
                .UseSubscribeConfiguration(cfg => cfg
                    .Consume(x => x.WithPrefetchCount(_configuration.PrefetchCount)
                        .WithRoutingKey(routingKey))
                    .FromDeclaredQueue(x => x.WithName($"{_configuration.QueueName}:{routingKey.ToLower()}")
                        .WithArgument(QueueArgument.DeadLetterExchange, "dlx"))
                    .OnDeclaredExchange(x => x.WithName(_configuration.ExchangeName)));
    }
}