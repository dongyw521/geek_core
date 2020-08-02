using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MIddleWareDemo.ExceptionHandler;
using MIddleWareDemo.Filter;
using MIddleWareDemo.Middleware;

namespace MIddleWareDemo
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

            //第三种异常处理，在mvc的体系中获并处理异常
            services.AddMvc(options=> {
                options.Filters.Add<MyExceptionFilter>();
                //options.Filters.Add<MyExceptionFilterAttribute>();//attribute也可以注册到全局，因为实现了IExceptionFilter接口
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            //第一种和第二种都是在整个请求管道里处理异常
            //第一种异常处理中间件
            //app.UseExceptionHandler("/error");//异常捕获中间件，跳转到错误页

            //第二种异常处理中间件，阻断器方式，输出json格式错误信息
            //app.UseExceptionHandler(appBuilder =>
            //{
            //    appBuilder.Run(async context =>
            //    {
            //        var feature = context.Features.Get<IExceptionHandlerFeature>();
            //        var err = feature?.Error;
            //        var knownException = err as IKnownException;
            //        if (knownException == null)
            //        {
            //            var logger = app.ApplicationServices.GetService<ILogger<MyExceptionFilter>>();
            //            logger.LogError(err, err.Message);
            //            knownException = KnownException.UnKnown;
            //            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            //        }
            //        else
            //        {
            //            knownException = KnownException.FromKnownException(knownException);
            //            context.Response.StatusCode = StatusCodes.Status200OK;
            //        }

            //        //var jsonOptions = app.ApplicationServices.GetService<IOptions<JsonOptions>>();
            //        context.Response.ContentType = "application/json charset=utf-8";

            //        await context.Response.WriteAsync(JsonSerializer.Serialize(knownException));
            //    });
            //});

            //中间件,请求处理
            app.Use(async (context, next) => {
                //await context.Response.WriteAsync("Hello my middleware!");//已经response.write，后续无法修改header
                await next();
                await context.Response.WriteAsync("Hello my another middleware!");//不匹配任何特定的url,全局中间件
            });

            //匹配某种url，才使用这个中间件
            app.Map("/abc", abcBuilder => {
                abcBuilder.Use(async (context, next) => {
                    await next();
                    await context.Response.WriteAsync("abc");//先输出abc，再输出hello my another middleware
                });
            });

            //MapWhen更精确的配置url
            app.MapWhen(context => {
                return context.Request.Query.Keys.Contains("mid");
            }, appBuilder => {
                //Run阻断器，不是严格的中间件，后续不再执行
                appBuilder.Run(async context => {
                    await context.Response.WriteAsync("stop...");//会输出stop...Hello my another middleware!
                });
            });

            app.UseMyMiddleware();//扩展方法方式注册自己的中间件

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
