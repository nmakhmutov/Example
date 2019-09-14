﻿using System;
using System.IO;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Elwark.EventBus.RabbitMQ
{
    public class RabbitMQPersistentConnection : IRabbitMQPersistentConnection
    {
        private readonly string _clientProviderName;
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitMQPersistentConnection> _logger;
        private readonly int _retryCount;
        private readonly object _syncRoot = new object();
        private IConnection _connection;
        private bool _disposed;

        public RabbitMQPersistentConnection(IConnectionFactory connectionFactory,
            ILogger<RabbitMQPersistentConnection> logger, string clientProviderName, int retryCount = 5)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _clientProviderName = clientProviderName ?? throw new ArgumentNullException(nameof(clientProviderName));
            _retryCount = retryCount;
        }

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        public bool TryConnect()
        {
            _logger.LogInformation("RabbitMQ Client is trying to connect");

            lock (_syncRoot)
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(_retryCount,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (ex, time) => _logger.LogWarning(ex,
                            "RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})",
                            $"{time.TotalSeconds:n1}", ex.Message)
                    );

                policy.Execute(() => _connection = _connectionFactory.CreateConnection(_clientProviderName));

                if (IsConnected)
                {
                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    _connection.CallbackException += OnCallbackException;
                    _connection.ConnectionBlocked += OnConnectionBlocked;

                    _logger.LogInformation(
                        "RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events",
                        _connection.Endpoint.HostName);

                    return true;
                }

                _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

                return false;
            }
        }

        public IModel CreateModel() => IsConnected
            ? _connection.CreateModel()
            : throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            Close();
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed)
                return;
            
            _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");
            
            Close();
            TryConnect();
        }

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed)
                return;
            
            _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");
            
            Close();
            TryConnect();
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (_disposed)
                return;
            
            _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");
            
            Close();
            TryConnect();
        }

        private void Close()
        {
            try
            {
                _connection.Close();
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }
    }
}