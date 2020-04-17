using System;
using System.Threading;
using System.Threading.Tasks;

namespace Elwark.EventBus.Logging.EF
{
    public interface IIntegrationEventLogService
    {
        Task<IntegrationEventLogEntry> GetAsync(Guid id, CancellationToken cancellationToken = default);
        Task SaveEventAsync(IntegrationEvent evt, CancellationToken cancellationToken = default);
        Task MarkEventAsPublishedAsync(Guid eventId, CancellationToken cancellationToken = default);
        Task MarkEventAsInProgressAsync(Guid eventId, CancellationToken cancellationToken = default);
        Task MarkEventAsFailedAsync(Guid eventId, CancellationToken cancellationToken = default);
    }
}