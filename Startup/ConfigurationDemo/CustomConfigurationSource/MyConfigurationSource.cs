using System;
using Microsoft.Extensions.Configuration;

namespace ConfigurationDemo.CustomConfigurationSource
{
    class MyConfigurationSource : IConfigurationSource
    {

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new MyConfigurationProvider();
        }
    }
}
