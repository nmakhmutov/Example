﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Elwark.EventBus.Abstractions;

namespace Elwark.EventBus.Logging.EF
{
    public interface IIntegrationEventLogService
    {
        Task<IntegrationEventLogEntry> RetrieveEventLogPendingToPublishAsync(Guid id,
            CancellationToken cancellationToken = default);

        Task SaveEventAsync(IntegrationEvent evt);
        Task MarkEventAsPublishedAsync(Guid eventId);
        Task MarkEventAsInProgressAsync(Guid eventId);
        Task MarkEventAsFailedAsync(Guid eventId);
    }
}