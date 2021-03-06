﻿using System;
using System.Linq;
using System.Reflection;
using System.Text;
using AddinEngine.Abstract;
using Microsoft.Extensions.Configuration;

namespace AddinEngine
{
    public static class ConfigurationLoader
    {
        public static void LoadDependencies(this IConfigurationBuilder configurationBuilder, string path,
            string pattern)
        {
            try
            {
                var modules = Loader<IConfigurationResolver>.GetModules(path, pattern);
                var registerComponent = new ConfigurationRegister(configurationBuilder);
                foreach (var module in modules.Where(module => module != null))
                {
                    module.SetUp(registerComponent);
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
