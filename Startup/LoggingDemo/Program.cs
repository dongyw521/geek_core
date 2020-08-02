using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.Console;

namespace LoggingDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            //configBuilder.AddCommandLine(args);
            var config = configBuilder.Build();

            IServiceCollection serviceCollection = new ServiceCollection();
            //serviceCollection.AddSingleton<IConfiguration>(config);//自定义实例不受容器管理
            serviceCollection.AddSingleton<IConfiguration>(p => config);//利用工厂模式使得config受容器的管理。

            serviceCollection.AddTransient<OrderService>();//和add的顺序无关，AddLogging可以在Service的下面

            serviceCollection.AddLogging(builder => {
                builder.AddConfiguration(config.GetSection("Logging"));//配置文件里Console是Logging的子节点
                builder.AddConsole();
            });

            
            var serviceProvider = serviceCollection.BuildServiceProvider();

            #region 采用Logger名称方式
            //var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            //var logger = loggerFactory.CreateLogger("MyLogger");//通过logger名称来创建logger

            //logger.LogDebug(2001, "Debug");//EventId
            //logger.LogError("Error");
            //Exception ex = new Exception("出错了");
            //logger.LogError(ex, "error");
            #endregion

            #region 采用强类型ILogger<>，推荐
            //OrderService的日志记录
            //var order = serviceProvider.GetService<OrderService>();
            //order.Show();//通过ILogger的泛型注册模板来获取logger
            #endregion

            #region 日志作用域，用于调用链记日志情况
            ILogger pLogger = serviceProvider.GetService<ILogger<Program>>();
            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
                using (pLogger.BeginScope("ScopeId:{scopeid}", Guid.NewGuid()))//支持配置热更新
                {
                    pLogger.LogInformation("Info日志");
                    pLogger.LogDebug("Debug日志");
                    pLogger.LogError("Error日志");
                }
                System.Threading.Thread.Sleep(100);//异步原因会先输出分割线，强制让线程休息下
                Console.WriteLine("=========分割线=========");
            }


            #endregion

            //Console.ReadKey();
            Console.WriteLine("Hello World!");
        }
    }
}
