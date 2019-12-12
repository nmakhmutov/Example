using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elwark.EventBus;
using Elwark.EventBus.RabbitMq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Elwark.EventBus.Sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddElwarkRabbitMq(Configuration.GetSection("MessageQueue").Get<ElwarkRabbitConfiguration>())
                .AddEventHandler<TestEvent, TestEventHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IIntegrationEventPublisher publisher)
        {
            foreach (var i in Enumerable.Range(0, 10))
            {
                publisher.PublishAsync(new TestEvent {Name = $"test message {i}"}).GetAwaiter().GetResult();    
            }
            
        }
    }

    public class TestEvent : IntegrationEvent
    {
        public string Name { get; set; }
    }

    public class TestEventHandler : IIntegrationEventHandler<TestEvent>
    {
        private readonly ILogger<TestEventHandler> _logger;

        public TestEventHandler(ILogger<TestEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(TestEvent evt, CancellationToken cancellationToken)
        {
            throw new ArgumentException();
            _logger.LogInformation(JsonConvert.SerializeObject(evt));
            await Task.CompletedTask;
        }
    }
}