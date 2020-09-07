using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MediatRDemo
{
    public class Program
    {
        async static Task Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddMediatR(typeof(Program).Assembly);//扫描程序集，关联IRquest,IRequestHandler

            var provider = services.BuildServiceProvider();
            var mediatR = provider.GetService<IMediator>();

            //CQRS命令查询职责分离
            await mediatR.Send(new MyCommand() { itemName = "test" });

            //领域事件
            await mediatR.Publish(new MyEvent("testEvent"));



            Console.WriteLine("Hello World!");



        }
    }

    #region CQRS
    public class MyCommand : IRequest<long>
    {
        public string itemName { get; set; }
    }

    /// <summary>
    /// 只执行一个Handler,代码顺序在前面的执行
    /// </summary>
    public class MyCommandHandlerV2 : IRequestHandler<MyCommand, long>
    {
        public Task<long> Handle(MyCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine("MyCommandHandlerV2:" + request.itemName);
            return Task.FromResult(10L);
        }
    }

    public class MyCommandHandler : IRequestHandler<MyCommand, long>
    {
        public Task<long> Handle(MyCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine("MyCommandHandler:" + request.itemName);
            return Task.FromResult(10L);
        }
    }
    #endregion

    #region 领域事件处理
    public class MyEvent : INotification
    {
        public MyEvent(string _itemContent) => itemContent = _itemContent;

        public string itemContent { get; private set; }
    }

    /// <summary>
    /// 两个Handler会都执行
    /// </summary>
    public class MyEventHandler : INotificationHandler<MyEvent>
    {
        public Task Handle(MyEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine("MyEventHandler:" + notification.itemContent);
            return Task.CompletedTask;
        }
    }

    public class MyEventHandlerV2 : INotificationHandler<MyEvent>
    {
        public Task Handle(MyEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine("MyEventHandlerV2:" + notification.itemContent);
            return Task.CompletedTask;
        }
    }

    #endregion

}
