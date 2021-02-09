using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace AddinEngine
{
    internal static class Loader<T>
    {
        public static IEnumerable<T> GetModules(string path, string pattern)
        {
            var dirCat = new DirectoryCatalog(path, pattern);
            var importDef = BuildImportDefinition();
            using var aggregateCatalog = new AggregateCatalog();
            aggregateCatalog.Catalogs.Add(dirCat);

            using var compositionContainer = new CompositionContainer(aggregateCatalog);
            var exports = compositionContainer.GetExports(importDef);

            var modules = exports
                .Where(export => export.Definition.ContractName == typeof(T).FullName)
                .Select(export => (T)export.Value);

            foreach (var module in modules.Where(module => module != null))
            {
                //yield return to make the module process outside this function before dispose aggregateCatalog
                yield return module;
            }
        }

        private static ImportDefinition BuildImportDefinition()
        {
            return new ImportDefinition(  
                def => true, typeof(T).FullName, ImportCardinality.ZeroOrMore, false, false);  
        }
    }
}