using System.Threading;
using System.Threading.Tasks;
using RawRabbit;
using RawRabbit.Enrichers.MessageContext;

namespace Elwark.EventBus.RabbitMq
{
    public class RabbitMqEventPublisher : IIntegrationEventPublisher
    {
        private readonly IBusClient _busClient;
        private readonly ElwarkRabbitConfiguration _configuration;

        public RabbitMqEventPublisher(IBusClient busClient, ElwarkRabbitConfiguration configuration)
        {
            _busClient = busClient;
            _configuration = configuration;
        }

        public Task PublishAsync(IntegrationEvent evt, CancellationToken cancellationToken) =>
            _busClient.PublishAsync(evt,
                ctx =>
                    ctx.UseMessageContext(new ElwarkMessageContext
                        {
                            GlobalRequestId = evt.Id
                        })
                        .UsePublishConfiguration(cfg =>
                            cfg.WithRoutingKey(evt.GetType().Name)
                                .OnExchange(_configuration.ExchangeName)
                        ), cancellationToken);
    }
}