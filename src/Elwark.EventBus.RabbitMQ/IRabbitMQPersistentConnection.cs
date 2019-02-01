using System;
using RabbitMQ.Client;

namespace Elwark.EventBus.RabbitMQ
{
    // ReSharper disable once InconsistentNaming
    public interface IRabbitMQPersistentConnection
        : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}