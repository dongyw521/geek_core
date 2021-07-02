using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dyw.Ordering.Api.Applications.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dyw.Ordering.Api.Controllers
{
    /// <summary>
    /// 订单
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        IMediator mediator;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="_mediator"></param>
        public OrderController(IMediator _mediator)
        {
            mediator = _mediator;
        }

        /// <summary>
        /// 测试action
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Test()
        {
            return Content("OrderController");
        }

        /// <summary>
        /// 创建order
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<long> CreateOrder([FromBody]CreateOrderCommand cmd)
        {
            return await mediator.Send(cmd, HttpContext.RequestAborted);
        }
    }
}
