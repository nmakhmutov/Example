using System;

namespace Elwark.EventBus
{
    public class IntegrationEvent
    {
        public IntegrationEvent() =>
            (Id, CreationDate) = (Guid.NewGuid(), DateTime.UtcNow);

        public Guid Id { get; }
        
        public DateTime CreationDate { get; }
    }
}