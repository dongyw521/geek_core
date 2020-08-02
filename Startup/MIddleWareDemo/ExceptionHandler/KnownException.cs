using System;
namespace MIddleWareDemo.ExceptionHandler
{
    public class KnownException : IKnownException
    { 
        public string Code { get; private set; }
        public string Msg { get; private set; }
        public object[] Data { get; private set; }

        public readonly static IKnownException UnKnown = new KnownException { Code = "999", Msg = "未知错误", Data = null };

        public static IKnownException FromKnownException(IKnownException knownException)
        {
            return new KnownException { Code = knownException.Code, Msg = knownException.Msg, Data = knownException.Data };
        }
    }
}
