using System;
using Microsoft.AspNetCore.Builder;

namespace MIddleWareDemo.Middleware
{
    public static class MyMiddlewareExtension
    {
        
        public static void UseMyMiddleware(this IApplicationBuilder app)
        {
            //app.UseMiddleware(typeof(MyMiddleware));
            app.UseMiddleware<MyMiddleware>();//效果一样
        }
    }
}
