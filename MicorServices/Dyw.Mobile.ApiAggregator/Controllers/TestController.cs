using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrpcServiceDemo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dyw.Mobile.ApiAggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        OrderingGrpc.OrderingGrpcClient _client;

        public TestController(OrderingGrpc.OrderingGrpcClient client)
        {
            _client = client;
        }

        [HttpGet]
        public IActionResult Abc()
        {
            return Content("Dyw.Mobile.ApiAggregator");
        }

        [HttpGet("ordering")]
        public async Task<IActionResult> Order(string uname)
        {
            var reply = await _client.SayHelloAsync(new HelloRequest() { Name = uname });
            return Content(reply.Message);
        }
    }
}
