using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HttpClientDemo.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttpClientDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        OrderServiceClient client;

        public OrderController(OrderServiceClient _client)
        {
            client = _client;
        }

        [HttpGet("Get")]
        public async Task<string> Get()
        {
            return await client.Get();
        }

        [HttpGet("NamedGet")]
        public async Task<string> NamedGet([FromServices]NamedOrderServiceClient _client)
        {
            return await _client.Get();
        }

        [HttpGet("TypedGet")]
        public async Task<string> TypedGet([FromServices] TypedOrderServiceClient _client)
        {
            return await _client.Get();
        }
    }
}
