using Dyw.Domain.Abstractions;
using Dyw.Ordering.Domain.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dyw.Ordering.Domain.Events
{
    public class OrderCreatedDomainEvent : IDomainEvent
    {
        public Order Order { get; private set; }

        public OrderCreatedDomainEvent(Order order)
        {
            Order = order;
        }
    }
}
