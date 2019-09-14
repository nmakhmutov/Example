namespace Elwark.EventBus.RabbitMQ
{
    public class RabbitMqSettings
    {
        public RabbitMqSettings(){}
        
        public RabbitMqSettings(string hostName, string username, string password, string virtualHost,
            string exchange, string queueName, ushort retryCount, string connectionName)
        {
            HostName = hostName;
            Username = username;
            Password = password;
            VirtualHost = virtualHost;
            Exchange = exchange;
            QueueName = queueName;
            RetryCount = retryCount;
            ConnectionName = connectionName;
        }

        public string HostName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public string Exchange { get; set; }
        public string QueueName { get; set; }
        public string ConnectionName { get; set; }
        public ushort RetryCount { get; set; }
    }
}