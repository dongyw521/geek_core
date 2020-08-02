using System;
using System.Timers;
using Microsoft.Extensions.Configuration;

namespace ConfigurationDemo.CustomConfigurationSource
{
    class MyConfigurationProvider : ConfigurationProvider
    {
        Timer myTimer;
        public MyConfigurationProvider():base()
        {
            myTimer = new Timer();
            myTimer.Interval = 3000;
            myTimer.Elapsed += MyTimer_Elapsed;
            myTimer.Start();
            
        }

        private void MyTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ReLoad(true);
        }

        public override void Load()
        {
            ReLoad(false);
        }

        public void ReLoad(bool reload)
        {
            //此处可以加载远程配置，比如阿波罗Kazoo
            this.Data["key1"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (reload)
            {
                base.OnReload();
            }
        }
    }
}
