using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AddinEngine
{
    public class AddinManager
    {
        public static string AddinFolderPath = null;
        static AddinManager()
        {
            Global.ConfigureAppConfiguration += Global_ConfigureAppConfiguration;
            Global.ConfigureServices += Global_ConfigureServices;
            Global.ConfigureWeb += Global_ConfigureWeb;
        }

        public static void WarmUp()
        {
            //Do nothing
        }

        private static void Global_ConfigureAppConfiguration(
            Microsoft.Extensions.Hosting.HostBuilderContext context,
            IConfigurationBuilder configurationBuilder)
        {
            var addInsDirectory = AddinFolderPath ?? GetApplicationRoot(true) + "/AddIns";

            foreach (var dir in Directory.GetDirectories(addInsDirectory))
            {
                configurationBuilder.LoadDependencies(dir, "*.dll");
            }
        }

        private static void Global_ConfigureServices(
            Microsoft.Extensions.Hosting.HostBuilderContext context,
            IServiceCollection services)
        {
            var addInsDirectory = AddinFolderPath ?? GetApplicationRoot(true) + "/AddIns";

            foreach (var dir in Directory.GetDirectories(addInsDirectory))
            {
                services.LoadDependencies(dir, "*.dll", context.Configuration);
            }

            //Loading in background
            // ThreadPool.QueueUserWorkItem(_ =>
            // {
            //     Console.WriteLine("log");
            //     services.LoadDependencies("./AddIns/GenericWebApiCommands", "*.dll");
            //     Console.WriteLine("Load");
            // });
        }

        private static void Global_ConfigureWeb(dynamic app, dynamic env)
        {
            var addInsDirectory = AddinFolderPath ?? GetApplicationRoot(true) + "/AddIns";

            foreach (var dir in Directory.GetDirectories(addInsDirectory))
            {
                ConfigureWebLoader.LoadDependencies(dir, "*.dll", app, env);
            }
        }

        private static string GetApplicationRoot(bool includeBinFolder)
        {
            var   exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher;
            appPathMatcher = includeBinFolder
                ? new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*")
                : new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)"); //(?=\\+bin): It’s a lookahead (zero length assertion). It will check for “bin” but not include it in the match
            var appRoot = appPathMatcher.Match(exePath).Value;
            if (string.IsNullOrEmpty(appRoot))
            {
                //linux cases or docker
                appRoot = ".";
            }
            return appRoot;
        }
    }
}
