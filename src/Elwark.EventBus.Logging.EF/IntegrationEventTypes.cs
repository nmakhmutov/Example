using System;
using System.Collections.Generic;

namespace Elwark.EventBus.Logging.EF
{
    public class IntegrationEventTypes
    {
        public IntegrationEventTypes(IReadOnlyCollection<Type> types)
        {
            Types = types ?? throw new ArgumentNullException(nameof(types));
        }

        public IReadOnlyCollection<Type> Types { get; }
    }
}