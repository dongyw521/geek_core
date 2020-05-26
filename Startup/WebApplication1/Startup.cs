using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApplication1.Services;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //services.AddSingleton<>//泛型注册方式

            //services.AddSingleton<I>(serviceProvider=>{
            //serviceProvider.GetService().....//工厂模式：注册类之间有相互依赖关系
            //})

            //尝试注册
            //services.TryAddSingleton<>//如果原来注册过，则注册失败，不管这次的实现是否和原来相同
            //services.TryAddEnumerable<>//如果原来注册过，且这次是不同的实现，则可以注册成功。

            //释放及替换
            //services.RemoveAll<>
            //services.Replace

            //注册泛型模板，适用于仓储模式
            //services.AddSingleton<typeof(IGeneric<>),typeof(Generic<>));

            //获取注册的服务实例方式：1.controller构造函数；2.方法中的参数标记[FromService]

            //services.AddSingleton<IOrderService, DisposableOrderService>();//全局单例，ctrl+c结束webapplication时，才会调用dispose方法释放，实例是注册到了根容器
            services.AddTransient<IOrderService, DisposableOrderService>();//瞬时模式，每次刷页面，处理完请求后都会释放该实例
            //scope模式，在每个scope里是一个单例，scope之间是不同的实例
            //services.AddScoped<IOrderService>(sp =>
            //{
            //    return new DisposableOrderService();
            //});

            //用户自己创建实例
            //var service = new DisposableOrderService();
            //services.AddSingleton<IOrderService>(service);//注册用户自己创建的实例,结束webapplication时,容器没有释放该实例，只能等待GC回收
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //从根容器获取瞬时服务，只有退出应用程序时才会被释放掉，会出现瞬时服务的堆积，违背了瞬时服务的初衷
            var s = app.ApplicationServices.GetService<IOrderService>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
