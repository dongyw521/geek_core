using System;
using ConfigurationDemo.CustomConfigurationSource;

namespace Microsoft.Extensions.Configuration
{
    public static class MyConfigurationBuilderExtension
    {
        /// <summary>
        /// 通过扩展方法把自定义的ConfigurationSource暴露出去，自己的类定义为internal
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IConfigurationBuilder AddMyConfigurationSource(this IConfigurationBuilder builder)
        {
            builder.Add(new MyConfigurationSource());
            return builder;
        }
    }
}
