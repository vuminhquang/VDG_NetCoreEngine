using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AddinEngine
{
    public static class Global
    {
        public delegate void NotifyConfigureConfigs(HostBuilderContext context, IConfigurationBuilder configs); // delegate
        public static event NotifyConfigureConfigs ConfigureAppConfiguration;
        public delegate void NotifyConfigureServices(HostBuilderContext context, IServiceCollection services); // delegate
        public static event NotifyConfigureServices ConfigureServices;

        // For web only
        public delegate void NotifyConfigureWeb(dynamic app, dynamic env);
        public static event NotifyConfigureWeb StartupConfigure;
        public delegate void NotifyConfigureEndpoint(dynamic endpoints, dynamic env);
        public static event NotifyConfigureEndpoint ConfigureEndpoints;

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
                    ConfigureAppConfiguration?.Invoke(context, config);
                    //Su dung trong cac class:
                    //Configuration["CoreCrawler:OnlyCrawlUrlBeginWith"]
                })
                .ConfigureServices(ConfiguringServices)
                // .ConfigureWebHostDefaults(webBuilder =>
                // {
                //     webBuilder.UseStartup<Startup>();
                // })
                ;
        }
        
        private static void ConfiguringServices(HostBuilderContext hostBuilderContext, IServiceCollection services)
        {
            ConfigureServices?.Invoke(hostBuilderContext, services);
        }

        /// <summary>
        /// This function should be called inside Configure method of Startup class, using only in case of webapp
        /// Called before configuring Endpoints, need to call this manually
        /// Attention: If multiple Configure method calls exist, the last Configure call is used.
        /// while Multiple calls to ConfigureServices append to one another.
        /// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup?view=aspnetcore-5.0
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        /// <param name="env">IWebHostEnvironment</param>
        public static void Startup_Configure(dynamic app, dynamic env)
        {
            StartupConfigure?.Invoke(app, env);
        }

        /// <summary>
        /// Should be called inside app.UseEndpoints
        /// Need to call this manually
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public static void Startup_ConfigureEndpoints(dynamic endpoints, dynamic env)
        {
            ConfigureEndpoints?.Invoke(endpoints, env);
        }
    }
}
