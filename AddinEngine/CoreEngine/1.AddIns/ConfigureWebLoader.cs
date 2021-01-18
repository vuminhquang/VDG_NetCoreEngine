using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;
using System.Text;
using AddinEngine.Abstract;
using AddInEngine.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AddinEngine
{
    public static class ConfigureWebLoader
    {
        public static void LoadDependencies(string path, string pattern, dynamic app, dynamic env)
        {
            var dirCat    = new DirectoryCatalog(path, pattern);  
            var importDef = BuildImportDefinition();  
            try
            {
                using var aggregateCatalog = new AggregateCatalog();
                aggregateCatalog.Catalogs.Add(dirCat);

                using var compositionContainer = new CompositionContainer(aggregateCatalog);
                var exports = compositionContainer.GetExports(importDef);  
  
                var modules = exports
                        .Where(export => export.Definition.ContractName == typeof(IWebConfigurationResolver).FullName)
                        .Select(export => export.Value as IWebConfigurationResolver);
  
                // var registerComponent = new DependencyRegister(serviceCollection);  
                foreach (var module in modules.Where(module => module != null))  
                {  
                    module.SetUp(app, env);  
                }
            }  
            catch (ReflectionTypeLoadException typeLoadException)  
            {  
                var builder = new StringBuilder();  
                foreach (var loaderException in typeLoadException.LoaderExceptions)  
                {  
                    builder.AppendFormat("{0}\n", loaderException.Message);  
                }  
  
                throw new TypeLoadException(builder.ToString(), typeLoadException);  
            }  
        }  
  
        private static ImportDefinition BuildImportDefinition()  
        {  
            return new ImportDefinition(  
                def => true, typeof(IDependencyResolver).FullName, ImportCardinality.ZeroOrMore, false, false);  
        }  
    }
}
