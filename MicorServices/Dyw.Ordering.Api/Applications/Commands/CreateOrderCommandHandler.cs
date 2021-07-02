using Dyw.Ordering.Domain.Entities.OrderAggregate;
using Dyw.Ordering.Infrastructure.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dyw.Ordering.Api.Applications.Commands
{
    /// <summary>
    /// 创建order处理逻辑
    /// </summary>
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, long>
    {
        IOrderRepository orderRepository;

        /// <summary>
        /// 构造函数注入仓储
        /// </summary>
        /// <param name="_orderRepository"></param>
        public CreateOrderCommandHandler(IOrderRepository _orderRepository)
        {
            orderRepository = _orderRepository;
        }

        /// <summary>
        /// 具体处理逻辑
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<long> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            Address address = new Address("test", "beijing", "010000");
            Order order = new Order("123", "dyw", request.ItemCount, address);

            await orderRepository.AddAsync(order);
            await orderRepository.UnitOfWork.SaveEntitiesAsync();
            return order.Id;
        }
    }
}
