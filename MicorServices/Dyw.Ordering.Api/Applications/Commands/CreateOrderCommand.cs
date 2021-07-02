using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dyw.Ordering.Api.Applications.Commands
{
    /// <summary>
    /// 创建order命令
    /// </summary>
    public class CreateOrderCommand : IRequest<long>
    {
        public long ItemCount { get; set; }

        public CreateOrderCommand(long itemCount)
        {
            ItemCount = itemCount;
        }
    }
}
