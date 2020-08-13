using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace RoutingDemo.RouteConstraint
{
    public class MyRouteConstraint : IRouteConstraint
    {
        public MyRouteConstraint()
        {
        }

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (RouteDirection.IncomingRequest == routeDirection)
            {
                var v = values[routeKey];
                if (long.TryParse(v.ToString(), out var value))
                {
                    return true;
                }

            }
            return false;
        }
    }
}
