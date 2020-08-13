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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceOptionsConfig.Services;

namespace ServiceOptionsConfig
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
            //services.Configure<MvcOptions>(mvcOptions => { 
            //    mvcOptions
            //});
            //services.AddSingleton<OrderServiceOptions>();
            //每一个service可以有自己的options
            //services.Configure<OrderServiceOptions>(Configuration.GetSection("OrderService"));//从配置来实例化OrderServiceOptions,这样就保证了service和配置的解耦
            //services.AddSingleton<IOrderService, OrderService>();
            //services.AddScoped<IOrderService, OrderService>();

            //通过扩展IServiceCollection简化OrderService的注册
            services.AddOrderService(Configuration.GetSection("OrderService"));
            services.AddControllers();
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
