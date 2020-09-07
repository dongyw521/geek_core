using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttpClientDemo.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HttpClientDemo
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
            services.AddHttpClient();//1.全局httpclient
            services.AddScoped<OrderServiceClient>();
            services.AddScoped<NamedOrderServiceClient>();
            services.AddSingleton<MsgHandler.RequestIdMessageHandler>();
            //services.AddScoped<TypedOrderServiceClient>();

            //2.命名httpclient
            services.AddHttpClient("namedClient", client =>
            {
                client.DefaultRequestHeaders.Add("dyw-name", "dyw");
                client.BaseAddress = new Uri("https://localhost:5002");
            }).AddHttpMessageHandler(provider => provider.GetService<MsgHandler.RequestIdMessageHandler>());

            //3.类型方式httpclient
            services.AddHttpClient<TypedOrderServiceClient>(client => client.BaseAddress = new Uri("https://localhost:5002"));//无需单独注册TypedOrderServiceClient
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
