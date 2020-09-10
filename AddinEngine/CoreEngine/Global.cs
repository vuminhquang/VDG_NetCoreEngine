using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AddinEngine
{
    public static class Global
    {
        public delegate void NotifyConfigureConfigs(HostBuilderContext context, IConfigurationBuilder configs); // delegate
        public static event NotifyConfigureConfigs ConfiguringConfigs;
        public delegate void NotifyConfigureServices(HostBuilderContext context, IServiceCollection services); // delegate
        public static event NotifyConfigureServices ConfiguringServices;

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            AddinManager.WarmUp();
            return Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(builder =>
                {
                    // builder.AddEnvironmentVariables();
                    // var root = GetApplicationRoot(true);
                    // builder.SetBasePath(root);
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false);
                    // var root = GetApplicationRoot(true);
                    // config.SetBasePath(root);
                    ConfiguringConfigs?.Invoke(context, config);
                    //Su dung trong cac class:
                    //Configuration["CoreCrawler:OnlyCrawlUrlBeginWith"]
                })
                .ConfigureServices(ConfigureServices)
                // .ConfigureWebHostDefaults(webBuilder =>
                // {
                //     webBuilder.UseStartup<Startup>();
                // })
                ;
        }

        private static void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services)
        {
            ConfiguringServices?.Invoke(hostBuilderContext, services);
        }
    }
}
