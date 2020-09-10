using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
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
            var dirCat    = new DirectoryCatalog(path, pattern);  
            var importDef = BuildImportDefinition();  
            try  
            {  
                using (var aggregateCatalog = new AggregateCatalog())  
                {  
                    aggregateCatalog.Catalogs.Add(dirCat);  
  
                    using (var componsitionContainer = new CompositionContainer(aggregateCatalog))  
                    {  
                        IEnumerable<Export> exports = componsitionContainer.GetExports(importDef);  
  
                        IEnumerable<IDependencyResolver> modules =  
                            exports
                                .Where(export => export.Definition.ContractName == typeof(IDependencyResolver).FullName)
                                .Select(export => export.Value as IDependencyResolver);
  
                        var registerComponent = new DependencyRegister(serviceCollection);  
                        foreach (IDependencyResolver module in modules.Where(module => module != null))  
                        {  
                            module.SetUp(registerComponent, configuration);  
                        }  
                    }  
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
  
        private static ImportDefinition BuildImportDefinition()  
        {  
            return new ImportDefinition(  
                def => true, typeof(IDependencyResolver).FullName, ImportCardinality.ZeroOrMore, false, false);  
        }  
    }
}
