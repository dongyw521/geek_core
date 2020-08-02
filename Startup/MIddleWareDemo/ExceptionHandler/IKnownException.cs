using System;
namespace MIddleWareDemo.ExceptionHandler
{
    public interface IKnownException
    {
        string Code { get; }

        string Msg { get; }

        Object[] Data { get; }
    }
}
