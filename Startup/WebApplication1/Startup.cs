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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
