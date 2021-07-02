using Dyw.Domain.Abstractions;
using Dyw.Ordering.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dyw.Ordering.Domain.Entities.OrderAggregate
{
    public class Order : Entity<long>, IAggregateRoot
    {
        public string UserId { get; private set; }

        public string UserName { get; private set; }

        public long ItemCount { get; private set; }

        public Address Address { get; private set; }

        public Order() { }

        public Order(string userId, string userName, long itemCount, Address address)
        {
            UserId = userId;
            UserName = userName;
            ItemCount = itemCount;
            Address = address;

            this.AddDomainEvent(new OrderCreatedDomainEvent(this));
        }

        public void ChangeAddress(Address address)
        {
            Address = address; 
        }
    }
}
