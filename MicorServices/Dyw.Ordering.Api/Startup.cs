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
                builder.UseMySql(Configuration.GetValue<string>("Mysql"), b => b.MigrationsAssembly("Dyw.Ordering.Api"));//cli�����з�ʽǨ����Ҫָ��Ǩ�ƣ�����dbcontext���ĳ���
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

                //����Ȩ��С��
                swagger.OperationFilter<AddResponseHeadersFilter>();
                swagger.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                //��header�����token�����ݵ���̨
                swagger.OperationFilter<SecurityRequirementsOperationFilter>();
                swagger.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���)ֱ���������������Bearer {token}(ע������֮����һ���ո�) ",
                    Name = "Authorization",//jwtĬ�ϵĲ�������
                    In = ParameterLocation.Header,//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
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
            //     �˷�����ʹ��Ǩ�����������ݿ�.����,�޷�ʹ��Ǩ�Ƹ��´��������ݿ�.
            //     ���Ҫ��λ��ϵ���ݿⲢʹ��Ǩ��,����ʹ��DbContext.Database.Migrate()������ȷ���������ݿ�,��Ӧ������Ǩ��
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
                //endpoints.MapDynamicControllerRoute<>();//3.0������̬����·�ɣ������ڶ������л�����
                //webapi�²���Ч
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
