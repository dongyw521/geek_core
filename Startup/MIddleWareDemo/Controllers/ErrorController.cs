using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MIddleWareDemo.ExceptionHandler;

namespace MIddleWareDemo.Controllers
{
    //[AllowAnonymous]
    public class ErrorController: Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 异常处理页面，需要addMvc()才可以跳转到视图
        /// </summary>
        /// <returns></returns>
        [Route("/error")]
        public IActionResult Index()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var err = feature?.Error;
            var knownException = err as IKnownException;

            if (knownException == null)
            {
                //var logger = HttpContext.RequestServices.GetService<ILogger<ErrorController>>();
                _logger.LogError(err, err.Message);
                knownException = KnownException.UnKnown;
            }
            else
            {
                knownException = KnownException.FromKnownException(knownException);
            }
            return View(knownException);
                
        }
    }
}
