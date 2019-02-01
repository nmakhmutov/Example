using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Elwark.EventBus.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Elwark.EventLog.EF
{
    public class IntegrationEventLogService : IIntegrationEventLogService
    {
        private readonly IntegrationEventLogContext _integrationEventLogContext;

        public IntegrationEventLogService(DbConnection dbConnection)
        {
            if (dbConnection is null)
                throw new ArgumentNullException(nameof(dbConnection));

            _integrationEventLogContext = new IntegrationEventLogContext(
                new DbContextOptionsBuilder<IntegrationEventLogContext>()
                    .UseNpgsql(dbConnection)
                    .EnableSensitiveDataLogging()
                    .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning))
                    .Options);
        }

        public Task SaveEventAsync(IntegrationEvent evt, DbTransaction transaction)
        {
            if (transaction is null)
                throw new ArgumentNullException(nameof(transaction),
                    $"A {typeof(DbTransaction).FullName} is required as a pre-requisite to save the evt.");

            if (_integrationEventLogContext.Database.CurrentTransaction is null)
                _integrationEventLogContext.Database.UseTransaction(transaction);

            var eventLogEntry = new IntegrationEventLogEntry(evt);
            _integrationEventLogContext.IntegrationEventLogs.Add(eventLogEntry);

            return _integrationEventLogContext.SaveChangesAsync();
        }

        public Task MarkEventAsPublishedAsync(IntegrationEvent evt)
        {
            var eventLogEntry = _integrationEventLogContext.IntegrationEventLogs.Single(ie => ie.EventId == evt.Id);
            eventLogEntry.TimesSent++;
            eventLogEntry.State = EventStateEnum.Published;

            _integrationEventLogContext.IntegrationEventLogs.Update(eventLogEntry);

            return _integrationEventLogContext.SaveChangesAsync();
        }
    }
}