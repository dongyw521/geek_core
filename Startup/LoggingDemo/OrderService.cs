using System;
using Microsoft.Extensions.Logging;
namespace LoggingDemo
{
    public class OrderService
    {
        ILogger<OrderService> _logger { get; set; } //通过ILogger的泛型注册模板来获取logger
        public OrderService(ILogger<OrderService> logger)
        {
            _logger = logger;
        }

        public void Show()
        {
            //...业务代码
            //默认走的是Console配置节点的Default日志级别，也可以自己配置，logger名称：LoggingDemo.OrderService
            _logger.LogInformation("{time}:这是OrderService的日志", DateTime.Now);//推荐采用模板消息，避免字符串拼接能耗
            //_logger.LogInformation($"{DateTime.Now}:这是OrderService的日志");//不推荐动态拼接，即便不输出日志也会拼接
        }
    }
}
