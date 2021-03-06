﻿using System;

namespace GridDomain.CQRS.Infrastructure
{
    /// <summary>
    /// Represents an event message.
    /// </summary>
    public interface ISourcedEvent
    {
        /// <summary>
        /// Gets the identifier of the source originating the event.
        /// </summary>
        Guid SourceId { get; } 

        Guid SagaId { get; }

        DateTime CreatedTime { get; }
    }
}