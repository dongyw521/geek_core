using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Extensions.Logging;
using Serilog.Sinks.SystemConsole.Themes;

namespace LoggingSeriLogDemo
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        public static void Main(string[] args)
        {
            //此处的日志配置用于webhost的启动，以及web应用的主体
            //此处已配置logger，所以appsettings.json中有没有serilog的配置节点都可以记录日志
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration)
                //.MinimumLevel.Debug()//注释掉则应用配置文件的日志级别
                .Enrich.FromLogContext()
                .Enrich.WithProperty("LogFlagCode", "MySerilog")
                .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}||{Level}||{LogFlagCode}] {SourceContext}||{Properties:j}||{Message:lj}||{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate)
                //.WriteTo.Console(new RenderedCompactJsonFormatter())
                .WriteTo.File(formatter: new RenderedCompactJsonFormatter(), "serilog\\myserilog.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            try
            {
                Log.Information("WebHost Start!");
                //CreateHost之前配置好Log.Logger,如上。
                CreateHostBuilder(args).Build().Run();
                //return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host start error!");
                //return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }

            //CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).UseSerilog();//使微软的ILogger使用Serilog
    }
}
