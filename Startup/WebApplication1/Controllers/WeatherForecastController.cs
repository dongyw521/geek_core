using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        //[HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    var rng = new Random();
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = rng.Next(-20, 55),
        //        Summary = Summaries[rng.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderService"></param>
        /// <param name="orderService2"></param>
        /// <returns></returns>
        /*
         单例模式注册时：IOrderService只有一个实例，并且只有ctrl+c结束webapplication时，才会调用dispose方法释放
         瞬时模式注册时：IOrderService共有四个实例，并且用完就释放
         作用域模式注册时：IOrderService共有两个实例，参数的那个两个是一个实例，方法体里那两个是一个实例。参数和方法体里是不同的作用域，一个action请求的大的容器，一个是请求创建的一个子容器。
        */
        [HttpGet]
        public int Get([FromServices] IOrderService orderService, [FromServices] IOrderService orderService2, [FromServices]IHostApplicationLifetime appLifetime, bool stop = false)//单例和scope两个orderService为一个实例；瞬时模式时，是两个不同的实例
        {
            //Console.WriteLine("========1=======");
            //using (IServiceScope scope = HttpContext.RequestServices.CreateScope())//创建一个scope就会给你一个新的实例
            //{
            //    var service = scope.ServiceProvider.GetService<IOrderService>();//与根容器是两个不同的实例
            //    var service2 = scope.ServiceProvider.GetService<IOrderService>();//service和service2是同一个实例
            //}
            //Console.WriteLine("========2=======");
            
            if (stop)
            {
                appLifetime.StopApplication();
            }
            Console.WriteLine("请求处理");

            return 1;
        }
    }
}
