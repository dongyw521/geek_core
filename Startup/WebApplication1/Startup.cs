using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
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

        /// <summary>
        /// autofac注册服务的入口
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            #region 普通注册
            //builder.RegisterType<MyService>().As<IMyService>();
            #endregion

            #region 命名方式注册
            //builder.RegisterType<MyService2>().Named<IMyService>("s2");
            #endregion

            #region 属性注册
            //builder.RegisterType<MyServiceName>();
            //builder.RegisterType<MyService2>().As<IMyService>().PropertiesAutowired();//允许我的属性可以依赖注入，而不是允许我自己可以被别的类拿过去当做属性注入
            #endregion

            #region AOP,不改变原有类，而在原有类执行方法的切面上，加入一些自己的方法
            //builder.RegisterType<MyInterceptor>();
            //builder.RegisterType<MyService2>().As<IMyService>().PropertiesAutowired().InterceptedBy(typeof(MyInterceptor)).EnableInterfaceInterceptors();
            #endregion

            #region 注册到特定命名的子容器
            builder.RegisterType<MyServiceName>().InstancePerMatchingLifetimeScope("myscope");
            #endregion

        }

        public ILifetimeScope AutofacContainer { get; private set; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //从根容器获取瞬时服务，只有退出应用程序时才会被释放掉，会出现瞬时服务的堆积，违背了瞬时服务的初衷
            //var s = app.ApplicationServices.GetService<IOrderService>();

            AutofacContainer = app.ApplicationServices.GetAutofacRoot();//获取autofac根容器

            #region autofac注册及获取服务演示
            //var myservice = AutofacContainer.Resolve<IMyService>();//获取到的是默认注册方式的MyService的实例
            //var myservice2 = AutofacContainer.ResolveNamed<IMyService>("s2");//获取的是命名方式注册的MyService2

            //myservice.ShowCode();
            //myservice2.ShowCode();
            #endregion

            #region 子容器获取服务，该子容器内是单例
            using(var _scope = AutofacContainer.BeginLifetimeScope("myscope"))
            {
                var service0 = _scope.Resolve<MyServiceName>();
                using var __scope = _scope.BeginLifetimeScope();//在创建子容器获取的都是一个实例
                var service1 = __scope.Resolve<MyServiceName>();
                Console.WriteLine($"service0==service1为：{service0 == service1}");//返回true

            }

            #endregion




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
