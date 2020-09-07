using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientDemo.MsgHandler
{
    public class RequestIdMessageHandler : DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("x-guid", new Guid().ToString());
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
