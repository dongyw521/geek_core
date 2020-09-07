using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpClientDemo.Client
{
    public class TypedOrderServiceClient
    {
        HttpClient _client;

        public TypedOrderServiceClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<string> Get()
        {
            return await _client.GetStringAsync("/weatherforecast");
        }
    }
}
