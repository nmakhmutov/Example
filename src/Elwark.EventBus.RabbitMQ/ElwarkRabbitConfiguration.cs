using System.Text;
using RawRabbit.Configuration;

namespace Elwark.EventBus.RabbitMq
{
    public class ElwarkRabbitConfiguration : RawRabbitConfiguration
    {
        public ElwarkRabbitConfiguration()
        {
            ExchangeName = string.Empty;
            QueueName = string.Empty;
            ConnectionName = string.Empty;
            Port = 5672;
            RetryCount = 5;
            PrefetchCount = 30;
        }

        public string ExchangeName { get; set; }

        public string QueueName { get; set; }

        public string ConnectionName { get; set; }

        public ushort RetryCount { get; set; }
        
        public ushort PrefetchCount { get; set; }

        public string GetConnectionString()
        {
            var sb = new StringBuilder("amqp://");

            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
                sb.Append($"{Username}:{Password}@");

            sb.Append($"{string.Join(",", Hostnames)}:{Port}/");

            if (!string.IsNullOrEmpty(VirtualHost) && VirtualHost[0] != '/')
                sb.Append($"{VirtualHost}");

            return sb.ToString();
        }
    }
}