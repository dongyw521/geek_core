using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpClientDemo.Client
{
    public class OrderServiceClient
    {
        IHttpClientFactory clientFac;

        public OrderServiceClient(IHttpClientFactory _clientFac)
        {
            clientFac = _clientFac;
        }

        public async Task<string> Get()
        {
            var _client = clientFac.CreateClient();
            return await _client.GetStringAsync("https://localhost:5002/weatherforecast");
        }
    }
}
