using System;
using Microsoft.Extensions.FileProviders;

namespace FileProviderDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            //物理文件提供程序
            IFileProvider provider1 = new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory);

            //foreach(var con in provider1.GetDirectoryContents("/"))
            //{
            //    Console.WriteLine(con.Name);
            //}

            //嵌入式文件提供程序
            IFileProvider provider2 = new EmbeddedFileProvider(typeof(Program).Assembly);

            IFileInfo embedFileInfo = provider2.GetFileInfo("q.html");

            //Console.WriteLine(embedFileInfo.Name);

            //组合文件提供程序
            IFileProvider provider3 = new CompositeFileProvider(provider1, provider2);
            foreach (var con in provider3.GetDirectoryContents("/"))
            {
                Console.WriteLine(con.Name);
            }
        }
    }
}
