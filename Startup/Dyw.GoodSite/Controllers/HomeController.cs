using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Dyw.GoodSite.Models;
using EasyCaching.Core;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace Dyw.GoodSite.Controllers
{
    [EnableCors("apiCors")]//所有action支持跨域
    [AutoValidateAntiforgeryToken]//自动验证所有post请求的antiforgerytoken，没有为非法请求
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 登录逻辑
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="returnurl"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(string user, string password, string returnurl)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim("username", user));
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            if (string.IsNullOrEmpty(returnurl))
            {
                return Content("登录成功");
            }
            else
            {
                var tmpUri = new Uri(returnurl);//跳转外站，验证是否合法，再跳转
                if (tmpUri.Host.Contains("baidu"))
                {
                    return Redirect(returnurl);
                }
                return LocalRedirect(returnurl);//用local防止重定向攻击
                //return Redirect(returnurl);//直接跳转容易发生开放重定向攻击
            }
        }

        /// <summary>
        /// 防止跨站请求伪造演示
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]//header不携带X-CSRF-TOKEN信息，为非法请求
        public IActionResult CreateOrder(long itemId,long count)
        {
            return Content("itemId=" + itemId + ",count=" + count);
        }

        /// <summary>
        /// 测试跨域访问
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [EnableCors("apiCors")]//该action启用跨域
        public object PostCors(string name)
        {
            return new { name = name };
        }

        /// <summary>
        /// 测试ResponseCache缓存
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [DisableCors]//标记该action不支持跨域
        [ResponseCache(Duration = 60, VaryByQueryKeys = new string[] { "query" })]
        public OrderModel GetOrder([FromQuery] string query)
        {
            return new OrderModel { Id = 10, DateStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
        }

        /// <summary>
        /// 测试分布式缓存
        /// </summary>
        /// <param name="easyCache"></param>
        /// <param name="cache"></param>
        /// <param name="memCache"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public IActionResult GetDis([FromServices] IEasyCachingProvider easyCache, [FromServices] IDistributedCache cache, [FromServices] IMemoryCache memCache, [FromQuery] string query)
        {
            //easyCache
            //string key = $"dis-{query ?? ""}";
            //var result = easyCache.Get(key, () => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), TimeSpan.FromSeconds(60)).Value;
            //return Content(result);

            //DistributedCache
            string key2 = $"dis-{query ?? ""}";
            var result = cache.GetString(key2);
            if (string.IsNullOrEmpty(result))
            {
                result = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                cache.SetString(key2, result, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60) });
            }
            return Content(result);
        }
    }
}
