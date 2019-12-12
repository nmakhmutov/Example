using System.Threading;
using System.Threading.Tasks;

namespace Elwark.EventBus
{
    public interface IIntegrationEventHandler<in TIntegrationEvent>
    {
        Task HandleAsync(TIntegrationEvent evt, CancellationToken cancellationToken = default);
    }
}