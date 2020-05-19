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
            //services.AddSingleton<>//����ע�᷽ʽ

            //services.AddSingleton<I>(serviceProvider=>{
            //serviceProvider.GetService().....//����ģʽ��ע����֮�����໥������ϵ
            //})

            //����ע��
            //services.TryAddSingleton<>//���ԭ��ע�������ע��ʧ�ܣ�������ε�ʵ���Ƿ��ԭ����ͬ
            //services.TryAddEnumerable<>//���ԭ��ע�����������ǲ�ͬ��ʵ�֣������ע��ɹ���

            //�ͷż��滻
            //services.RemoveAll<>
            //services.Replace

            //ע�᷺��ģ�壬�����ڲִ�ģʽ
            //services.AddSingleton<typeof(IGeneric<>),typeof(Generic<>));

            //��ȡע��ķ���ʵ����ʽ��1.controller���캯����2.�����еĲ������[FromService]
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
