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
    public static class ConfigurationLoader
    {  
        public static void LoadDependencies(this IConfigurationBuilder configurationBuilder, string path, string pattern)  
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
  
                        IEnumerable<IConfigurationResolver> modules =  
                            exports
                                .Where(export => export.Definition.ContractName == typeof(IConfigurationResolver).FullName)
                                .Select(export => export.Value as IConfigurationResolver);
  
                        var registerComponent = new ConfigurationRegister(configurationBuilder);  
                        foreach (IConfigurationResolver module in modules.Where(module => module != null))
                        {  
                            module.SetUp(registerComponent);
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
                def => true, typeof(IConfigurationResolver).FullName, ImportCardinality.ZeroOrMore, false, false);  
        }  
    }
}
