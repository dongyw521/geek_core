using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace ConfigurationDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            IConfigurationBuilder builder = new ConfigurationBuilder();

            //1.从内存注入配置信息
            //builder.AddInMemoryCollection(new Dictionary<string, string> {
            //    {"key1","value1" },
            //    {"key2","value2" },
            //    {"section1:key1","value1" },
            //    {"section1:section2:key3","value3" }//逐层嵌套
            //});

            //IConfigurationRoot configurationRoot = builder.Build();

            //Console.WriteLine(configurationRoot["key1"]);
            //Console.WriteLine(configurationRoot["key2"]);

            //IConfigurationSection section = configurationRoot.GetSection("section1");
            //Console.WriteLine(section["key1"]);
            //IConfigurationSection section2 = section.GetSection("section2");
            //Console.WriteLine($"key3={section2["key3"]}");

            //2.命令行配置模式，类似敲命令，后面带参数，用于部署控制台程序到linux或者docker
            //双横杠指定参数名称，参数值用空格或者等号；单横杠表示参数的别名（取决于SwitchMapping里的定义）
            //启动参数添加：commandlinekey1=123 --commandlinekey2=456 /commandlinekey3=789 -k1=k1
            //builder.AddCommandLine(args);

            //#region 命令替换模式
            //var mapper = new Dictionary<string, string> { {"-k1", "commandlinekey1" } };
            //builder.AddCommandLine(args, mapper);//k1会替换linekey1,相当于别名
            //#endregion


            //IConfigurationRoot configurationRoot = builder.Build();
            //Console.WriteLine(configurationRoot["commandlinekey1"]);//替换后会输出k1
            //Console.WriteLine(configurationRoot["commandlinekey2"]);
            //Console.WriteLine(configurationRoot["commandlinekey3"]);
            //Console.WriteLine(configurationRoot["k1"]);

            //3.操作系统环境变量获取配置信息（基于docker和k8容器技术的隔离策略才能实现），定义环境变量时，section分层的话要使用双下划__线作为层键
            //builder.AddEnvironmentVariables();//注入所有环境变量
            //IConfigurationRoot configurationRoot = builder.Build();

            //Console.WriteLine($"KEY1={configurationRoot["KEY1"]}");
            //Console.WriteLine($"KEY2={configurationRoot["KEY2"]}");
            //Console.WriteLine($"Section1KEY3={configurationRoot["Section1:KEY3"]}");//获取值是层键还是使用冒号
            //Console.WriteLine($"KEY4={configurationRoot["Dyw_KEY4"]}");

            builder.AddEnvironmentVariables("Dyw_");//只注入指定前缀的环境变量,注入时，会去掉前缀
            IConfigurationRoot configurationRoot1 = builder.Build();

            Console.WriteLine($"KEY1={configurationRoot1["KEY1"]}");//key1不存在
            Console.WriteLine($"KEY4={configurationRoot1["KEY4"]}");

        }
    }
}
