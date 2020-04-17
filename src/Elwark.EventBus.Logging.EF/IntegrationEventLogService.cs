using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Elwark.EventBus.Logging.EF
{
    public class IntegrationEventLogService : IIntegrationEventLogService
    {
        private readonly IntegrationEventLogContext _context;

        public IntegrationEventLogService(IntegrationEventLogContext context) =>
            _context = context ?? throw new ArgumentNullException(nameof(context));

        public Task<IntegrationEventLogEntry> GetAsync(Guid id, CancellationToken cancellationToken) =>
            _context.IntegrationEventLogs.SingleOrDefaultAsync(x => x.EventId == id, cancellationToken);
        
        public Task SaveEventAsync(IntegrationEvent evt, CancellationToken cancellationToken)
        {
            if (_context.Database.CurrentTransaction is null)
                _context.Database.BeginTransaction();

            var eventLogEntry = new IntegrationEventLogEntry(evt);
            _context.IntegrationEventLogs.Add(eventLogEntry);

            return _context.SaveChangesAsync(cancellationToken);
        }

        public Task MarkEventAsPublishedAsync(Guid eventId, CancellationToken cancellationToken) =>
            UpdateEventStatus(eventId, EventStateEnum.Published, cancellationToken);

        public Task MarkEventAsInProgressAsync(Guid eventId, CancellationToken cancellationToken) =>
            UpdateEventStatus(eventId, EventStateEnum.InProgress, cancellationToken);

        public Task MarkEventAsFailedAsync(Guid eventId, CancellationToken cancellationToken) =>
            UpdateEventStatus(eventId, EventStateEnum.PublishedFailed, cancellationToken);

        private Task UpdateEventStatus(Guid eventId, EventStateEnum status, CancellationToken cancellationToken)
        {
            var eventLogEntry = _context.IntegrationEventLogs.Single(ie => ie.EventId == eventId);
            eventLogEntry.State = status;

            if (status == EventStateEnum.InProgress)
                eventLogEntry.TimesSent++;

            _context.IntegrationEventLogs.Update(eventLogEntry);

            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}