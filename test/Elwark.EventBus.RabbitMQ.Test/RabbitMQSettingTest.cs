using System.Collections.Generic;
using Elwark.EventBus.RabbitMq;
using Xunit;

namespace Elwark.EventBus.RabbitMQ.Test
{
    public class RabbitMqSettingTest
    {
        [Fact]
        public void Check_full_paramenters_success()
        {
            var setting = new ElwarkRabbitConfiguration
            {
                Hostnames = new List<string> {"host"},
                Username = "username",
                Password = "password",
                VirtualHost = "virtualhost",
            };

            var connection = setting.GetConnectionString();

            Assert.Equal("amqp://username:password@host:5672/virtualhost", connection);
        }

        [Fact]
        public void Check_without_credentials_success()
        {
            var setting = new ElwarkRabbitConfiguration
            {
                Hostnames = new List<string> {"host"},
                VirtualHost = "virtualhost",
            };

            var connection = setting.GetConnectionString();

            Assert.Equal("amqp://host:5672/virtualhost", connection);
        }

        [Fact]
        public void Check_without_credentials_and_virtual_host_success()
        {
            var setting = new ElwarkRabbitConfiguration
            {
                Hostnames = new List<string> {"host"},
            };

            var connection = setting.GetConnectionString();

            Assert.Equal("amqp://host:5672/", connection);
        }

        [Fact]
        public void Check_custom_port_success()
        {
            var setting = new ElwarkRabbitConfiguration
            {
                Hostnames = new List<string> {"host"},
                Username = "username",
                Password = "password",
                VirtualHost = "virtualhost",
                Port = 1111
            };

            var connection = setting.GetConnectionString();

            Assert.Equal("amqp://username:password@host:1111/virtualhost", connection);
        }
    }
}