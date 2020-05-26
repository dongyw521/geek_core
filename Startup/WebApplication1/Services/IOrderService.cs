using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Services
{
    public interface IOrderService
    {

    }

    /// <summary>
    /// 实现了IDisposable接口，并由容器去创建实例，那么容器会根据生命周期判断，周期结束时，调用Dispose方法来释放对象。
    /// 不实现该接口，那么由GC来回收。
    /// 如果不是由容器创建的实例，不受容器的管理，需要用户自己释放
    /// </summary>
    public class DisposableOrderService : IOrderService, IDisposable
    {
        public void Dispose()
        {
            Console.WriteLine($"DisposableOrderService has been disposed:{this.GetHashCode()}");
            //throw new NotImplementedException();
        }
    }
}
