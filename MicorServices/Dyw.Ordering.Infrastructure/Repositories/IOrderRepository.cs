using Dyw.Infrastructure.Core;
using Dyw.Ordering.Domain.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dyw.Ordering.Infrastructure.Repositories
{
    public interface IOrderRepository : IRepository<Order, long>
    {

    }
}
