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
            #region 安全
            //1.跨站请求伪造
            //1.1不用cookie进行身份认证，采用header携带jwttoken；1.2采用antiforgery防止请求伪造；1.3尽量使用post方式操作业务数据
            services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");//自定义Antiforgery验证时需要携带的header的key

            //2.防止开放重定向
            //2.1 LocalRedirect(url)重定向，只能跳转本站；2.2 验证跳转host是否合法，再重定向


            //3.跨站脚本攻击
            //添加认证，默认为cookie认证
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
                    options.LoginPath = "/home/login";
                    options.Cookie.HttpOnly = true;//js脚本无法获取cookie值，防止跨站脚本攻击，最根本的方法还是验证用户输入内容，不让脚本代码注入进来
                });

            #endregion

            services.AddControllersWithViews();

            //开启所有post请求的antiforgerytoken验证
            services.AddMvc(options => {
                options.Filters.Add(new ValidateAntiForgeryTokenAttribute());
            });

            #region 缓存
            //1.内存缓存
            services.AddMemoryCache();

            //2.微软自带分布式缓存
            services.AddStackExchangeRedisCache(op =>
            {
                //op.InstanceName = "";
                Configuration.GetSection("RedisCache").Bind(op);
            });

            //3.输出缓存
            services.AddResponseCaching();

            //4.EasyCache缓存工具
            services.AddEasyCaching(op =>
            {
                op.UseRedis(Configuration, name: "EasyCaching");
            });
            #endregion

            #region 跨域策略
            //跨域请求时，第一次为跨域预检请求method=options，第二次才是真正的获取数据的请求
            services.AddCors(op =>
            {
                op.AddPolicy("apiCors", builder => builder
                .WithOrigins("https://localhost:5003")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                //暴露的返回头
                .WithExposedHeaders("abc"));

                //可以定义多个策略
                //op.AddPolicy("apiCors2",...)

                //op.AddDefaultPolicy()让所有action都可以跨域，执行定义的默认策略，不推荐这么使用


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
            app.UseCors();//跨域中间件
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
