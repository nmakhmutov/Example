using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Elwark.EventBus.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Elwark.EventBus.Logging.EF
{
    public class IntegrationEventLogService : IIntegrationEventLogService
    {
        private readonly IntegrationEventLogContext _context;
        private readonly List<Type> _eventTypes;

        public IntegrationEventLogService(IntegrationEventLogContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _eventTypes = Assembly.Load(Assembly.GetEntryAssembly().FullName)
                .GetTypes()
                .Where(t => t.Name.EndsWith(nameof(IntegrationEvent)))
                .ToList();
        }
        
        public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync() =>
            await _context.IntegrationEventLogs
                .Where(e => e.State == EventStateEnum.NotPublished)
                .OrderBy(o => o.CreationTime)
                .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t=> t.Name == e.EventTypeShortName)))
                .ToListAsync();

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

            if(status == EventStateEnum.InProgress)
                eventLogEntry.TimesSent++;

            _context.IntegrationEventLogs.Update(eventLogEntry);

            return _context.SaveChangesAsync();
        }
    }
}