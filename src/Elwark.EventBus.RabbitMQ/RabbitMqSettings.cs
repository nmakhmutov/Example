using System.Text;

namespace Elwark.EventBus.RabbitMQ
{
    public class RabbitMqSettings
    {
        public RabbitMqSettings()
        {
        }

        public RabbitMqSettings(string hostName, string username, string password, string virtualHost,
            string exchange, string queueName, ushort retryCount, string connectionName, int port = 5672)
        {
            HostName = hostName;
            Username = username;
            Password = password;
            VirtualHost = virtualHost;
            Exchange = exchange;
            QueueName = queueName;
            RetryCount = retryCount;
            ConnectionName = connectionName;
            Port = port;
        }

        public string HostName { get; set; }

        public int Port { get; set; } = 5672;
        
        public string Username { get; set; }
        
        public string Password { get; set; }
        
        public string VirtualHost { get; set; }
        
        public string Exchange { get; set; }
        
        public string QueueName { get; set; }
        
        public string ConnectionName { get; set; }
        
        public ushort RetryCount { get; set; }

        public string GetConnectionString()
        {
            var sb = new StringBuilder("amqp://");

            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password)) 
                sb.Append($"{Username}:{Password}@");

            sb.Append($"{HostName}:{Port}/");
            
            if (!string.IsNullOrEmpty(VirtualHost) && VirtualHost[0] == '/') 
                sb.Append($"{VirtualHost}");

            return sb.ToString();
        }
    }
}