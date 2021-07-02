using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dyw.Infrastructure.Core;
using Dyw.Ordering.Api.GrpcServices;
using Dyw.Ordering.Infrastructure;
using Dyw.Ordering.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Dyw.Ordering.Api
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
            services.AddMediatR(typeof(Domain.Entities.OrderAggregate.Order).Assembly, typeof(Program).Assembly);
            services.AddControllers().AddNewtonsoftJson();
            services.AddGrpc(options=> {
                options.EnableDetailedErrors = false;
            });
            services.AddDbContext<OrderingContext>(builder => {
                builder.UseMySql(Configuration.GetValue<string>("Mysql"), b => b.MigrationsAssembly("Dyw.Ordering.Api"));//cli命令行方式迁移需要指定迁移（包含dbcontext）的程序集
            });
            services.AddScoped<IOrderRepository, OrderRepository>();
            //services.AddScoped<typeof(IRepository<Entity>),typeof(Repository<TEntity,TDbContext>)>();

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecurityKey"]));
            services.AddSingleton(securityKey);

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    //options.LoginPath = "/api/account/login";
                    //options.Cookie.HttpOnly = true;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.FromSeconds(30),
                        ValidAudience = "localhost",
                        ValidIssuer = "localhost",
                        IssuerSigningKey = securityKey
                    };
                });

            services.AddSwaggerGen(swagger => {
                swagger.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Dyw OrderService Api", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                swagger.IncludeXmlComments(xmlPath);

                //开启权限小锁
                swagger.OperationFilter<AddResponseHeadersFilter>();
                swagger.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                //在header中添加token，传递到后台
                swagger.OperationFilter<SecurityRequirementsOperationFilter>();
                swagger.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传递)直接在下面框中输入Bearer {token}(注意两者之间是一个空格) ",
                    Name = "Authorization",//jwt默认的参数名称
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey
                });
            });
            

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //using(var scop = app.ApplicationServices.CreateScope())
            //{
            //    var dc = scop.ServiceProvider.GetService<OrderingContext>();
            //    /*
            //     此方法不使用迁移来创建数据库.此外,无法使用迁移更新创建的数据库.
            //     如果要定位关系数据库并使用迁移,可以使用DbContext.Database.Migrate()方法来确保创建数据库,并应用所有迁移
            //     */
            //    //dc.Database.EnsureCreated();
            //    dc.Database.Migrate();
            //}
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseMvc(routeBuilder => 
            //{
            //    routeBuilder.MapRoute(
            //        name: "default", 
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<OrderingService>();
                endpoints.MapControllers();
                //endpoints.MapDynamicControllerRoute<>();//3.0新增动态配置路由，适用于多语言切换环境
                //webapi下不生效
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );
            });

            app.UseSwagger();

            app.UseSwaggerUI(options => {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Dyw OrderService Api");
            });

        }
    }
}
