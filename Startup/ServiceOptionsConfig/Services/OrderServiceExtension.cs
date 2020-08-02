using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ServiceOptionsConfig.Services
{
    public static class OrderServiceExtension
    {
        public static IServiceCollection AddOrderService(this IServiceCollection services, IConfiguration configuration)
        {
            //services.Configure<OrderServiceOptions>(configuration);

            //给options添加验证
            //services.AddOptions<OrderServiceOptions>().Configure(options =>
            //{
            //    configuration.Bind(options);//options承载配置数据
            //}).Validate(options =>
            //{
            //    return options.MaxCount <= 100;
            //}, "MaxCount不能大于100");

            //注解验证方式
            //services.AddOptions<OrderServiceOptions>().Configure(options =>
            //{
            //    configuration.Bind(options);//options承载配置数据
            //}).ValidateDataAnnotations();

            //接口验证方式
            services.AddOptions<OrderServiceOptions>().Configure(options =>
            {
                configuration.Bind(options);//options承载配置数据
            }).Services.AddSingleton<IValidateOptions<OrderServiceOptions>, OrderServiceValidateOptions>();//下面这种也可以，只要注册了验证接口，会自动验证
            //services.AddSingleton<IValidateOptions<OrderServiceOptions>, OrderServiceValidateOptions>();


            //动态改变配置值
            //services.PostConfigure<OrderServiceOptions>(options => {
            //    options.MaxCount += 100;
            //});

            services.AddSingleton<IOrderService, OrderService>();
            return services;//返回IServiceCollection可以实现链式编程
        }
    }
}
