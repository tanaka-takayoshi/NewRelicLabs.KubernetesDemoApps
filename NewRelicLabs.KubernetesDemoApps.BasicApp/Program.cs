using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewRelic.LogEnrichers.NLog;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Web;

namespace NewRelicLabs.KubernetesDemoApps.BasicApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var loggerConfig = new LoggingConfiguration();
            var newReliTarget = new ConsoleTarget
            {
                Name = "NewRelicTarget",
                Layout = new NewRelicJsonLayout()
            };
            loggerConfig.AddTarget(newReliTarget);
            loggerConfig.AddRuleForAllLevels("NewRelicTarget");
            LogManager.Configuration = loggerConfig;
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                var host = CreateHostBuilder(args).Build();
                
                host.Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
