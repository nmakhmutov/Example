using System.Threading.Tasks;

namespace Elwark.EventBus.Abstractions
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
        where TIntegrationEvent : IntegrationEvent
    {
        Task Handle(TIntegrationEvent evt);
    }

    public interface IIntegrationEventHandler
    {
    }
}