using System;
using System.Linq;
using System.Text.Json.Serialization;
using Elwark.EventBus.Abstractions;

namespace Elwark.EventBus.Logging.EF
{
    public class IntegrationEventLogEntry
    {
        private IntegrationEventLogEntry() { }
        
        public IntegrationEventLogEntry(IntegrationEvent evt)
        {
            EventId = evt.Id;            
            CreationTime = evt.CreationDate;
            EventTypeName = evt.GetType().FullName;
            Content = JsonSerializer.ToString(evt);
            State = EventStateEnum.NotPublished;
            TimesSent = 0;
        }
        
        public Guid EventId { get; private set; }
        public string EventTypeName { get; private set; }
        public EventStateEnum State { get; set; }
        public int TimesSent { get; set; }
        public DateTime CreationTime { get; private set; }
        public string Content { get; private set; }
        
        
        public string EventTypeShortName => EventTypeName.Split('.')?.Last();
        
        public IntegrationEvent IntegrationEvent { get; private set; }

        public IntegrationEventLogEntry DeserializeJsonContent(Type type)
        {
            IntegrationEvent = JsonSerializer.Parse(Content, type) as IntegrationEvent;
            return this;
        }
    }
}