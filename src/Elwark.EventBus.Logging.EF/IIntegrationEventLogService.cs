using System;
using System.Threading;
using System.Threading.Tasks;

namespace Elwark.EventBus.Logging.EF
{
    public interface IIntegrationEventLogService
    {
        Task<IntegrationEventLogEntry> GetAsync(Guid id,
            CancellationToken cancellationToken = default);

        Task SaveEventAsync(IntegrationEvent evt);
        Task MarkEventAsPublishedAsync(Guid eventId);
        Task MarkEventAsInProgressAsync(Guid eventId);
        Task MarkEventAsFailedAsync(Guid eventId);
    }
}