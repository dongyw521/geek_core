using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dyw.Ordering.Api.Controllers
{
    /// <summary>
    /// 默认controller
    /// </summary>
    [Route("[controller]")]
    public class HomeController : Controller
    {
        /// <summary>
        /// 转向swagger
        /// </summary>
        /// <returns></returns>
        [Route("[action]")]
        [HttpGet]
        public IActionResult Index()
        {
            return Redirect("/swagger");
        }
    }
}
