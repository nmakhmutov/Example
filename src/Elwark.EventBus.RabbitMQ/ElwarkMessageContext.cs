using System;
using RawRabbit.Common;
using RawRabbit.Enrichers.MessageContext.Context;

namespace Elwark.EventBus.RabbitMq
{
    public class ElwarkMessageContext : IMessageContext
    {
        public Guid GlobalRequestId { get; set; }
        public RetryInformation RetryInformation { get; set; }
    }
}