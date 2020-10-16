using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Dyw.GoodSite
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
            #region ��ȫ
            //1.��վ����α��
            //1.1����cookie���������֤������headerЯ��jwttoken��1.2����antiforgery��ֹ����α�죻1.3����ʹ��post��ʽ����ҵ������
            services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");//�Զ���Antiforgery��֤ʱ��ҪЯ����header��key

            //2.��ֹ�����ض���
            //2.1 LocalRedirect(url)�ض���ֻ����ת��վ��2.2 ��֤��תhost�Ƿ�Ϸ������ض���


            //3.��վ�ű�����
            //�����֤��Ĭ��Ϊcookie��֤
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
                    options.LoginPath = "/home/login";
                    options.Cookie.HttpOnly = true;//js�ű��޷���ȡcookieֵ����ֹ��վ�ű�������������ķ���������֤�û��������ݣ����ýű�����ע�����
                });

            #endregion

            services.AddControllersWithViews();

            //��������post�����antiforgerytoken��֤
            services.AddMvc(options => {
                options.Filters.Add(new ValidateAntiForgeryTokenAttribute());
            });

            #region ����
            //1.�ڴ滺��
            services.AddMemoryCache();

            //2.΢���Դ��ֲ�ʽ����
            services.AddStackExchangeRedisCache(op =>
            {
                //op.InstanceName = "";
                Configuration.GetSection("RedisCache").Bind(op);
            });

            //3.�������
            services.AddResponseCaching();

            //4.EasyCache���湤��
            services.AddEasyCaching(op =>
            {
                op.UseRedis(Configuration, name: "EasyCaching");
            });
            #endregion

            #region �������
            //��������ʱ����һ��Ϊ����Ԥ������method=options���ڶ��β��������Ļ�ȡ���ݵ�����
            services.AddCors(op =>
            {
                op.AddPolicy("apiCors", builder => builder
                .WithOrigins("https://localhost:5003")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                //��¶�ķ���ͷ
                .WithExposedHeaders("abc"));

                //���Զ���������
                //op.AddPolicy("apiCors2",...)

                //op.AddDefaultPolicy()������action�����Կ���ִ�ж����Ĭ�ϲ��ԣ����Ƽ���ôʹ��


            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseResponseCaching();
            app.UseCors();//�����м��
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
