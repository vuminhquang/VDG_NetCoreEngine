﻿using System;
using System.Linq;
using System.Reflection;
using System.Text;
using AddinEngine.Abstract;

namespace AddinEngine
{
    public static class ConfigureWebLoader
    {
        public static void LoadDependencies(string path, string pattern, dynamic app, dynamic env)
        {
            try
            {
                var modules = Loader<IWebConfigurationResolver>.GetModules(path, pattern);
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
    }
}