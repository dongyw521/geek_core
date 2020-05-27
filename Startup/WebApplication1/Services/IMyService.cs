using System;
namespace WebApplication1.Services
{
    public interface IMyService
    {
        void ShowCode();
    }

    public class MyService : IMyService
    {
        public void ShowCode()
        {
            Console.WriteLine($"MyService Name is {GetHashCode()}");
        }
    }

    public class MyService2 : IMyService
    {
        public MyServiceName ServiceName { get; set; }

        public void ShowCode()
        {
            Console.WriteLine($"MyService2 Name is {GetHashCode()},ServiceName is null :{ServiceName == null}");
        }
    }

    public class MyServiceName
    {

    }
}
