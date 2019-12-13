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
        
        public Task SaveEventAsync(IntegrationEvent evt)
        {
            if (_context.Database.CurrentTransaction is null)
                _context.Database.BeginTransaction();

            var eventLogEntry = new IntegrationEventLogEntry(evt);
            _context.IntegrationEventLogs.Add(eventLogEntry);

            return _context.SaveChangesAsync();
        }

        public Task MarkEventAsPublishedAsync(Guid eventId) =>
            UpdateEventStatus(eventId, EventStateEnum.Published);

        public Task MarkEventAsInProgressAsync(Guid eventId) =>
            UpdateEventStatus(eventId, EventStateEnum.InProgress);

        public Task MarkEventAsFailedAsync(Guid eventId) =>
            UpdateEventStatus(eventId, EventStateEnum.PublishedFailed);

        private Task UpdateEventStatus(Guid eventId, EventStateEnum status)
        {
            var eventLogEntry = _context.IntegrationEventLogs.Single(ie => ie.EventId == eventId);
            eventLogEntry.State = status;

            if (status == EventStateEnum.InProgress)
                eventLogEntry.TimesSent++;

            _context.IntegrationEventLogs.Update(eventLogEntry);

            return _context.SaveChangesAsync();
        }
    }
}