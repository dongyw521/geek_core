using Dyw.Infrastructure.Core;
using Dyw.Ordering.Domain.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dyw.Ordering.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order, long, OrderingContext>, IOrderRepository
    {
        public OrderRepository(OrderingContext context) : base(context)
        {

        }
    }
}
