using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace RoutingDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController:ControllerBase
    {
        public OrderController()
        {
            
        }

        /// <summary>
        /// 订单是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:isLong}")]//自定义路由约束
        public bool OrderExist(long id)
        {
            return true;
        }

        /// <summary>
        /// 生成url
        /// </summary>
        /// <param name="id"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        [HttpGet("{id:max(20)}")]
        public string Max(long id,[FromServices]LinkGenerator link)
        {
            //相对路径
            var url = link.GetPathByAction(HttpContext, action: "Queue", controller: "Order", values: new { name = "abc" });

            //完整路径
            var url2 = link.GetUriByAction(HttpContext, action: "Queue", controller: "Order", values: new { name = "abc" });
            return url2;
        }

        /// <summary>
        /// 必传参数
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("{name:required}")]
        public bool Queue(string name)
        {
            return true;
        }

        /// <summary>
        /// 正则约束
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet("{number:regex(^\\d{{3}}$)}")]
        public bool Number(string number)
        {
            return true;
        }
    }
}
