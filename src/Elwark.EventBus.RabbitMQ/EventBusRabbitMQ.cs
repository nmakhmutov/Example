using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Elwark.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Elwark.EventBus.RabbitMQ
{
    // ReSharper disable once InconsistentNaming
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        private readonly string _exchangeName;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly int _retryCount;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventBusSubscriptionsManager _subsManager;

        private IModel _consumerChannel;
        private string _queueName;

        public EventBusRabbitMQ(
            IRabbitMQPersistentConnection persistentConnection,
            ILogger<EventBusRabbitMQ> logger,
            IEventBusSubscriptionsManager subsManager,
            IServiceProvider serviceProvider,
            string exchangeName,
            string queueName = null,
            int retryCount = 5)
        {
            _persistentConnection =
                persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _exchangeName = exchangeName ?? throw new ArgumentNullException(nameof(exchangeName));
            _queueName = queueName;
            _consumerChannel = CreateConsumerChannel();
            _retryCount = retryCount;
            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }

        public void Dispose()
        {
            _consumerChannel?.Dispose();

            _subsManager.Clear();
        }

        public void Publish(IntegrationEvent evt)
        {
            if (!_persistentConnection.IsConnected)
                _persistentConnection.TryConnect();

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, time) => _logger.LogWarning(ex, ex.Message));

            using (var channel = _persistentConnection.CreateModel())
            {
                var eventName = evt.GetType().Name;

                channel.ExchangeDeclare(_exchangeName, "direct");

                var message = JsonConvert.SerializeObject(evt);
                var body = Encoding.UTF8.GetBytes(message);

                policy.Execute(() =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2; // persistent

                    channel.BasicPublish(_exchangeName, eventName, true, properties, body);
                });
            }
        }

        public void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();
            DoInternalSubscription(eventName);
            _subsManager.AddSubscription<T, TH>();
        }

        public void SubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            DoInternalSubscription(eventName);
            _subsManager.AddDynamicSubscription<TH>(eventName);
        }

        public void UnsubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler =>
            _subsManager.RemoveDynamicSubscription<TH>(eventName);

        public void Unsubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T> =>
            _subsManager.RemoveSubscription<T, TH>();

        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
                _persistentConnection.TryConnect();

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(_exchangeName, "direct");

            channel.QueueDeclare(_queueName, true, false, false, null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                if (_subsManager.IsEmpty)
                {
                    channel.BasicNack(ea.DeliveryTag, false, true);
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    return;
                }


                var eventName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body);

                await ProcessEvent(eventName, message);

                channel.BasicAck(ea.DeliveryTag, false);
            };

            channel.BasicConsume(_queueName, false, consumer);

            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };

            return channel;
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_subsManager.HasSubscriptionsForEvent(eventName))
                using (var scope = _serviceProvider.CreateScope())
                    foreach (var subscription in _subsManager.GetHandlersForEvent(eventName))
                        if (subscription.IsDynamic)
                        {
                            if (!(scope.ServiceProvider.GetService(subscription.HandlerType) is IDynamicIntegrationEventHandler handler))
                                continue;

                            await handler.Handle((dynamic) JObject.Parse(message));
                        }
                        else
                        {
                            var handler = (dynamic) scope.ServiceProvider.GetService(subscription.HandlerType);
                            if (handler is null)
                                continue;

                            var eventType = _subsManager.GetEventTypeByName(eventName);
                            var integrationEvent = JsonConvert.DeserializeObject(message, eventType);

                            await handler.Handle((dynamic) integrationEvent);
                        }
        }

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_persistentConnection.IsConnected)
                _persistentConnection.TryConnect();

            using (var channel = _persistentConnection.CreateModel())
            {
                channel.QueueUnbind(_queueName, _exchangeName, eventName);

                if (_subsManager.IsEmpty)
                {
                    _queueName = string.Empty;
                    _consumerChannel.Close();
                }
            }
        }

        private void DoInternalSubscription(string eventName)
        {
            if (!_subsManager.HasSubscriptionsForEvent(eventName))
            {
                if (!_persistentConnection.IsConnected)
                    _persistentConnection.TryConnect();

                using (var channel = _persistentConnection.CreateModel())
                    channel.QueueBind(_queueName, _exchangeName, eventName);
            }
        }
    }
}