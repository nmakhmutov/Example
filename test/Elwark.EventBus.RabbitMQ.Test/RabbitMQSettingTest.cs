using Xunit;

namespace Elwark.EventBus.RabbitMQ.Test
{
    public class RabbitMqSettingTest
    {
        [Fact]
        public void Check_full_paramenters_success()
        {
            var setting = new RabbitMqSettings(
                "host",
                "username",
                "password",
                "virtualhost",
                "exchange",
                "queue",
                5,
                "connectionname");

            var connection = setting.GetConnectionString();
            
            Assert.Equal("amqp://username:password@host:5672/virtualhost", connection);
        }

        [Fact]
        public void Check_without_credentials_success()
        {
            var setting = new RabbitMqSettings(
                "host",
                null,
                null,
                "virtualhost",
                "exchange",
                "queue",
                5,
                "connectionname");

            var connection = setting.GetConnectionString();
            
            Assert.Equal("amqp://host:5672/virtualhost", connection);
        }
        
        [Fact]
        public void Check_without_credentials_and_virtual_host_success()
        {
            var setting = new RabbitMqSettings(
                "host",
                null,
                null,
                null,
                "exchange",
                "queue",
                5,
                "connectionname");

            var connection = setting.GetConnectionString();
            
            Assert.Equal("amqp://host:5672/", connection);
        }
        
        [Fact]
        public void Check_custom_port_success()
        {
            var setting = new RabbitMqSettings(
                "host",
                "username",
                "password",
                "virtualhost",
                "exchange",
                "queue",
                5,
                "connectionname",
                1111);

            var connection = setting.GetConnectionString();
            
            Assert.Equal("amqp://username:password@host:1111/virtualhost", connection);
        }
    }
}