using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Dyw.Mobile.Gateway.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        /// <summary>
        /// 模拟登陆页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login()
        {
            return Content("请登录");
        }

        /// <summary>
        /// 登陆写cookie
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CookieLogin(string account)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim("username", account));
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            return await Task.FromResult(Content("登陆成功"));
        }

        /// <summary>
        /// jwt登陆方式
        /// </summary>
        /// <param name="securityKey"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> JwtLogin([FromServices]SymmetricSecurityKey securityKey,string account)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("username", account));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "localhost",
                audience: "localhost",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
                );

            var _result = new JwtSecurityTokenHandler().WriteToken(token);
            return await Task.FromResult(Content(_result));
        }
    }
}
