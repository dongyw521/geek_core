using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using MIddleWareDemo.ExceptionHandler;

namespace MIddleWareDemo.Filter
{
    public class MyExceptionFilter : IExceptionFilter
    {
        private ILogger<MyExceptionFilter> _logger;
        public MyExceptionFilter(ILogger<MyExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var ex = context.Exception;
            var knownException = ex as IKnownException;
            if (knownException == null)
            {
                //var logger = context.HttpContext.RequestServices.GetService<ILogger<MyExceptionFilter>>();
                _logger.LogError(ex, ex.Message);
                knownException = KnownException.UnKnown;
                context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
            else
            {
                knownException = KnownException.FromKnownException(knownException);
                context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
            }

            context.Result = new JsonResult(knownException) { ContentType = "application/json charset=utf-8" };
        }
    }
}
