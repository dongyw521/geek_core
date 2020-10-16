using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GrpcServiceDemo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Bulkhead;
using Polly.CircuitBreaker;

namespace GrpcClientDemo
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
            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);//允许使用不加密的http2协议
            services.AddControllers();
            services.AddGrpcClient<Greeter.GreeterClient>(options =>
            {
                options.Address = new Uri("https://localhost:5001");//该请求url使用的http2协议
            })
            //.ConfigurePrimaryHttpMessageHandler(provider => {
            //    var socket = new SocketsHttpHandler();
            //    socket.SslOptions.RemoteCertificateValidationCallback = (a, b, c, d) => true;//允许无效，自签名的证书
            //    return socket;
            //})
            #region polly内置的策略
            //.AddTransientHttpErrorPolicy(policy => policy.RetryAsync(20));//请求出错500,408时重试20次
            //.AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(20, i => TimeSpan.FromSeconds(i*2)));//每隔i*2秒重试一次，总共20次，一直到成功或者失败
            .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryForeverAsync(i => TimeSpan.FromSeconds(i * 2)));//一直重试直到成功
            #endregion

            #region 自定义重试策略
            var reg = services.AddPolicyRegistry();//策略注册器
            reg.Add("retryforever", Policy.HandleResult<HttpResponseMessage>(msg =>
            {
                return msg.StatusCode == HttpStatusCode.Created;
            }).RetryForeverAsync());

            #region 两种方式给httpclient添加策略

            //方式一
            services.AddHttpClient("orderclientv1").AddPolicyHandler(Policy.HandleResult<HttpResponseMessage>(msg =>
            {
                return msg.StatusCode == HttpStatusCode.Created;
            }).RetryForeverAsync());

            //方式二
            services.AddHttpClient("orderclientv1.1").AddPolicyHandlerFromRegistry("retryforever");

            #endregion

            //根据请求方法判断使用哪种策略
            services.AddHttpClient("orderclientv2").AddPolicyHandlerFromRegistry((reg, msg) =>
            {
                //根据请求方法判断使用哪种策略，策略中呢再根据返回结果来执行策略
                return msg.Method == HttpMethod.Get ? reg.Get<IAsyncPolicy<HttpResponseMessage>>("retryforever") : Policy.NoOpAsync<HttpResponseMessage>();
            });

            #endregion

            #region 自定义熔断策略
            services.AddHttpClient("orderclientv3").AddPolicyHandler(Policy<HttpResponseMessage>.Handle<HttpRequestException>().CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 10,//发生几次请求失败
                durationOfBreak: TimeSpan.FromSeconds(10),//熔断时长
                onBreak: (r, t) => { },//熔断时执行
                onReset: () => { },//恢复后执行
                onHalfOpen: () => { }//恢复前执行，打一部分流量到服务，看服务是否可用
                ));

            //高级熔断策略
            services.AddHttpClient("orderclientv4").AddPolicyHandler(Policy<HttpResponseMessage>.Handle<HttpRequestException>().AdvancedCircuitBreakerAsync(
                failureThreshold: 0.8,//失败比例
                samplingDuration: TimeSpan.FromSeconds(10),//采样时间
                minimumThroughput: 100,//采样时间内最少请求数
                durationOfBreak: TimeSpan.FromSeconds(20),//熔断时长
                onBreak: (r, t) => { },
                onReset: () => { },
                onHalfOpen: () => { }
                ));

            #endregion

            #region 自定义熔断组合策略
            //熔断策略
            var breakPolicy = Policy<HttpResponseMessage>.Handle<HttpRequestException>().AdvancedCircuitBreakerAsync(
                failureThreshold: 0.8,//失败比例
                samplingDuration: TimeSpan.FromSeconds(10),//采样时间
                minimumThroughput: 100,//采样时间内最少请求数
                durationOfBreak: TimeSpan.FromSeconds(20),//熔断时长
                onBreak: (r, t) => { },
                onReset: () => { },
                onHalfOpen: () => { }
                );

            //熔断后还有继续的请求会抛出熔断异常，通过fallback策略处理后，返回友好结果
            var fallbackMsg = new HttpResponseMessage()
            {
                Content = new StringContent("{}")
            };
            var fallbackPolicy = Policy<HttpResponseMessage>.Handle<BrokenCircuitException>().FallbackAsync(fallbackMsg);

            //重试策略
            var retryPolicy = Policy.HandleResult<HttpResponseMessage>(msg =>
             {
                 return msg.StatusCode == HttpStatusCode.Created;
             }).WaitAndRetryAsync(5, (i) => TimeSpan.FromSeconds(i * 2));

            //组合策略
            var wrapPolicy = Policy.WrapAsync(fallbackPolicy, retryPolicy, breakPolicy);
            services.AddHttpClient("orderclientv4").AddPolicyHandler(wrapPolicy);

            #endregion

            #region 自定义限流降级组合策略

            //限流策略
            var bulkPolicy = Policy.BulkheadAsync<HttpResponseMessage>(
                maxParallelization: 30,//最高并发数
                maxQueuingActions: 20,//队列数
                onBulkheadRejectedAsync: (ctx) => Task.CompletedTask//限流时的处理
                );

            var fallbackMsg2 = new HttpResponseMessage()
            {
                Content = new StringContent("{}")
            };
            //限流后，且队列已满，继续请求会有限流的异常，通过fallback处理
            var fallbackPolicy2 = Policy<HttpResponseMessage>.Handle<BulkheadRejectedException>().FallbackAsync(fallbackMsg2);

            var wrapPolicy2 = Policy.WrapAsync(fallbackPolicy2, bulkPolicy);
            services.AddHttpClient("orderclientv5").AddPolicyHandler(wrapPolicy2);
            #endregion

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
