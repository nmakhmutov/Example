using System.Threading;
using System.Threading.Tasks;

namespace Elwark.EventBus
{
    public interface IIntegrationEventPublisher
    {
        Task PublishAsync(IntegrationEvent evt, CancellationToken cancellationToken = default);
    }
}