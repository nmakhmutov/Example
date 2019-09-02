using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elwark.EventBus.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Elwark.EventBus.Logging.EF
{
    public class IntegrationEventLogService : IIntegrationEventLogService
    {
        private readonly IntegrationEventLogContext _context;
        private readonly IntegrationEventTypes _eventTypes;

        public IntegrationEventLogService(IntegrationEventLogContext context, IntegrationEventTypes eventTypes)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _eventTypes = eventTypes ?? throw new ArgumentNullException(nameof(eventTypes));
        }

        public async Task<IReadOnlyCollection<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(
            CancellationToken cancellationToken) => await _context.IntegrationEventLogs
            .Where(e => e.State == EventStateEnum.NotPublished)
            .OrderBy(o => o.CreationTime)
            .Select(e =>
                e.DeserializeJsonContent(_eventTypes.Types.FirstOrDefault(t => t.Name == e.EventTypeShortName)))
            .ToArrayAsync(cancellationToken);

        public async Task<IntegrationEventLogEntry> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await _context.IntegrationEventLogs
                .SingleOrDefaultAsync(x => x.EventId == id, cancellationToken);

            return result?.DeserializeJsonContent(
                _eventTypes.Types.FirstOrDefault(x => x.Name == result.EventTypeShortName));
        }

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