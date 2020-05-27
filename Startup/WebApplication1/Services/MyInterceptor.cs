using System;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;

namespace WebApplication1.Services
{
    /// <summary>
    /// 拦截器
    /// </summary>
    public class MyInterceptor : IInterceptor
    {

        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine($"执行原类方法{invocation.Method.Name}前");
            invocation.Proceed();
            Console.WriteLine($"执行原类方法{invocation.Method.Name}后");
        }
    }
}
