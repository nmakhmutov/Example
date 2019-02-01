using System;
using Elwark.EventBus.Abstractions;
using Newtonsoft.Json;

namespace Elwark.EventLog.EF
{
    public class IntegrationEventLogEntry
    {
        private IntegrationEventLogEntry() { }

        public IntegrationEventLogEntry(IntegrationEvent evt)
        {
            EventId = evt.Id;
            CreationTime = evt.CreationDate;
            EventTypeName = evt.GetType().FullName;
            Content = JsonConvert.SerializeObject(evt, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            State = EventStateEnum.NotPublished;
            TimesSent = 0;
        }
        
        public Guid EventId { get; private set; }
        
        public string EventTypeName { get; private set; }
        
        public EventStateEnum State { get; set; }
        
        public int TimesSent { get; set; }
        
        public DateTime CreationTime { get; private set; }
        
        public string Content { get; private set; }
    }
}