using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dyw.Domain.Abstractions
{
    public interface IDomainEventHandler<TDomainEvent> : INotificationHandler<TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
    }
}
