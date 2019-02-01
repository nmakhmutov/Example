using System.Data.Common;
using System.Threading.Tasks;
using Elwark.EventBus.Abstractions;

namespace Elwark.EventLog.EF
{
    public interface IIntegrationEventLogService
    {
        Task SaveEventAsync(IntegrationEvent evt, DbTransaction transaction);
        
        Task MarkEventAsPublishedAsync(IntegrationEvent evt);
    }
}