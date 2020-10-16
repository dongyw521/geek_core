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
            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);//����ʹ�ò����ܵ�http2Э��
            services.AddControllers();
            services.AddGrpcClient<Greeter.GreeterClient>(options =>
            {
                options.Address = new Uri("https://localhost:5001");//������urlʹ�õ�http2Э��
            })
            //.ConfigurePrimaryHttpMessageHandler(provider => {
            //    var socket = new SocketsHttpHandler();
            //    socket.SslOptions.RemoteCertificateValidationCallback = (a, b, c, d) => true;//������Ч����ǩ����֤��
            //    return socket;
            //})
            #region polly���õĲ���
            //.AddTransientHttpErrorPolicy(policy => policy.RetryAsync(20));//�������500,408ʱ����20��
            //.AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(20, i => TimeSpan.FromSeconds(i*2)));//ÿ��i*2������һ�Σ��ܹ�20�Σ�һֱ���ɹ�����ʧ��
            .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryForeverAsync(i => TimeSpan.FromSeconds(i * 2)));//һֱ����ֱ���ɹ�
            #endregion

            #region �Զ������Բ���
            var reg = services.AddPolicyRegistry();//����ע����
            reg.Add("retryforever", Policy.HandleResult<HttpResponseMessage>(msg =>
            {
                return msg.StatusCode == HttpStatusCode.Created;
            }).RetryForeverAsync());

            #region ���ַ�ʽ��httpclient��Ӳ���

            //��ʽһ
            services.AddHttpClient("orderclientv1").AddPolicyHandler(Policy.HandleResult<HttpResponseMessage>(msg =>
            {
                return msg.StatusCode == HttpStatusCode.Created;
            }).RetryForeverAsync());

            //��ʽ��
            services.AddHttpClient("orderclientv1.1").AddPolicyHandlerFromRegistry("retryforever");

            #endregion

            //�������󷽷��ж�ʹ�����ֲ���
            services.AddHttpClient("orderclientv2").AddPolicyHandlerFromRegistry((reg, msg) =>
            {
                //�������󷽷��ж�ʹ�����ֲ��ԣ����������ٸ��ݷ��ؽ����ִ�в���
                return msg.Method == HttpMethod.Get ? reg.Get<IAsyncPolicy<HttpResponseMessage>>("retryforever") : Policy.NoOpAsync<HttpResponseMessage>();
            });

            #endregion

            #region �Զ����۶ϲ���
            services.AddHttpClient("orderclientv3").AddPolicyHandler(Policy<HttpResponseMessage>.Handle<HttpRequestException>().CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 10,//������������ʧ��
                durationOfBreak: TimeSpan.FromSeconds(10),//�۶�ʱ��
                onBreak: (r, t) => { },//�۶�ʱִ��
                onReset: () => { },//�ָ���ִ��
                onHalfOpen: () => { }//�ָ�ǰִ�У���һ�������������񣬿������Ƿ����
                ));

            //�߼��۶ϲ���
            services.AddHttpClient("orderclientv4").AddPolicyHandler(Policy<HttpResponseMessage>.Handle<HttpRequestException>().AdvancedCircuitBreakerAsync(
                failureThreshold: 0.8,//ʧ�ܱ���
                samplingDuration: TimeSpan.FromSeconds(10),//����ʱ��
                minimumThroughput: 100,//����ʱ��������������
                durationOfBreak: TimeSpan.FromSeconds(20),//�۶�ʱ��
                onBreak: (r, t) => { },
                onReset: () => { },
                onHalfOpen: () => { }
                ));

            #endregion

            #region �Զ����۶���ϲ���
            //�۶ϲ���
            var breakPolicy = Policy<HttpResponseMessage>.Handle<HttpRequestException>().AdvancedCircuitBreakerAsync(
                failureThreshold: 0.8,//ʧ�ܱ���
                samplingDuration: TimeSpan.FromSeconds(10),//����ʱ��
                minimumThroughput: 100,//����ʱ��������������
                durationOfBreak: TimeSpan.FromSeconds(20),//�۶�ʱ��
                onBreak: (r, t) => { },
                onReset: () => { },
                onHalfOpen: () => { }
                );

            //�۶Ϻ��м�����������׳��۶��쳣��ͨ��fallback���Դ���󣬷����Ѻý��
            var fallbackMsg = new HttpResponseMessage()
            {
                Content = new StringContent("{}")
            };
            var fallbackPolicy = Policy<HttpResponseMessage>.Handle<BrokenCircuitException>().FallbackAsync(fallbackMsg);

            //���Բ���
            var retryPolicy = Policy.HandleResult<HttpResponseMessage>(msg =>
             {
                 return msg.StatusCode == HttpStatusCode.Created;
             }).WaitAndRetryAsync(5, (i) => TimeSpan.FromSeconds(i * 2));

            //��ϲ���
            var wrapPolicy = Policy.WrapAsync(fallbackPolicy, retryPolicy, breakPolicy);
            services.AddHttpClient("orderclientv4").AddPolicyHandler(wrapPolicy);

            #endregion

            #region �Զ�������������ϲ���

            //��������
            var bulkPolicy = Policy.BulkheadAsync<HttpResponseMessage>(
                maxParallelization: 30,//��߲�����
                maxQueuingActions: 20,//������
                onBulkheadRejectedAsync: (ctx) => Task.CompletedTask//����ʱ�Ĵ���
                );

            var fallbackMsg2 = new HttpResponseMessage()
            {
                Content = new StringContent("{}")
            };
            //�������Ҷ���������������������������쳣��ͨ��fallback����
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
