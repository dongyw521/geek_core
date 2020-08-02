using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MIddleWareDemo.Middleware
{
    public class MyMiddleware
    {
        ILogger<MyMiddleware> _logger;
        RequestDelegate _next;

        public MyMiddleware(ILogger<MyMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (_logger.BeginScope("myLoggerScope"))
            {
                _logger.LogDebug("我的myMiddlewareExtension start!");
                await _next(context);
                _logger.LogDebug("我的myMiddlewareExtension end!");
            }
        }
    }
}
