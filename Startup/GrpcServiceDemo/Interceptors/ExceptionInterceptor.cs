using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcServiceDemo.Interceptors
{
    public class ExceptionInterceptor:Interceptor
    {
        public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return base.UnaryServerHandler(request, context, continuation);
            }
            catch(Exception ex)
            {
                var data = new Metadata();
                data.Add("message", ex.Message);
                throw new RpcException(new Status(StatusCode.Unknown, "unknown"), data);
            }
        }
    }
}
