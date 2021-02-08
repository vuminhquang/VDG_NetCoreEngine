using System;
using System.Linq;
using System.Reflection;
using System.Text;
using AddinEngine.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AddinEngine
{
    public static class DependencyLoader
    {  
        public static void LoadDependencies(this IServiceCollection serviceCollection, string path, string pattern, IConfiguration configuration)  
        {
            try
            {
                var modules = Loader<IDependencyResolver>.GetModules(path, pattern);
                var registerComponent = new DependencyRegister(serviceCollection);
                foreach (IDependencyResolver module in modules.Where(module => module != null))
                {
                    module.SetUp(registerComponent, configuration);
                }
            }
            catch (ReflectionTypeLoadException typeLoadException)
            {
                var builder = new StringBuilder();
                foreach (Exception loaderException in typeLoadException.LoaderExceptions)
                {
                    builder.AppendFormat("{0}\n", loaderException.Message);
                }

                throw new TypeLoadException(builder.ToString(), typeLoadException);
            }
        }
    }
}
