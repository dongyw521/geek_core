using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace ServiceOptionsConfig.Services
{
    public class OrderService : IOrderService
    {
        //OrderServiceOptions _options;
        //IOptions<OrderServiceOptions> _options;//使用IOptions使OrderServiceOptions可以与配置文件关联，只有首次加载读取配置
        //IOptionsSnapshot<OrderServiceOptions> _options;//对应services为scope模式时，每次请求都获取一次配置
        IOptionsMonitor<OrderServiceOptions> _options;//对应services为单例模式时，每次请求都获取一次配置

        //public OrderService(IOptions<OrderServiceOptions> options)
        //{
        //    _options = options;
        //}

        //public OrderService(IOptionsSnapshot<OrderServiceOptions> options)
        //{
        //    _options = options;
        //}

        public OrderService(IOptionsMonitor<OrderServiceOptions> options)
        {
            _options = options;
            //IOptionsMonitor有监听options变化的事件
            _options.OnChange((ops) =>
            {
                //_ = ops.MaxCount;
                Console.WriteLine($"新的配置值： {ops.MaxCount}");
            });
        }

        public void ShowMaxCount()
        {
            //Console.WriteLine($"MaxCount={_options.MaxCount}");
            //Console.WriteLine($"MaxCount={_options.Value.MaxCount}");//IOpitons通过value获取配置键值
            Console.WriteLine($"MaxCount={_options.CurrentValue.MaxCount}");//IOpitons通过value获取配置键值,monitor时用currentvalue
        }
    }

    /// <summary>
    /// service的选项配置,可以为每个service配一个options
    /// </summary>
    public class OrderServiceOptions
    {
        //[Range(1,20)]
        public int MaxCount { get; set; } = 100;
    }

    /// <summary>
    /// 添加验证接口
    /// </summary>
    public class OrderServiceValidateOptions : IValidateOptions<OrderServiceOptions>
    {
        public ValidateOptionsResult Validate(string name, OrderServiceOptions options)
        {
            if (options.MaxCount > 100)
            {
                return ValidateOptionsResult.Fail("MaxCount不能大于100哦！");
            }
            else
            {
                return ValidateOptionsResult.Success;
            }
        }
    }
}
