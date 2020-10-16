using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcServiceDemo;

namespace Dyw.Ordering.Api.GrpcServices
{
    public class OrderingService:OrderingGrpc.OrderingGrpcBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            //return base.SayHello(request, context);
            return Task.FromResult(new HelloReply
            {
                Message = "Hello" + request.Name
            });
        }
    }
}
