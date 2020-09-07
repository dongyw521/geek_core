using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpClientDemo.Client
{
    public class NamedOrderServiceClient
    {
        IHttpClientFactory _clientFactory;

        public NamedOrderServiceClient(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<string> Get()
        {
            var client = _clientFactory.CreateClient("namedClient");
            return await client.GetStringAsync("/weatherforecast");
        }
    }
}
