using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AddinEngine.Abstract._2.Helpers
{
    public static class Helper
    {
        public static IEnumerable<Type> GetAllTypesThatImplementInterface<T>(this Assembly assembly)
        {
            return assembly
                .GetTypes()
                .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface);
        }

        public static bool IsTypeRegistered<T>(this IDependencyRegister services, Type type)
        {
            var tDescriptor = services.FirstOrDefault(s => s.ServiceType == type);
            return tDescriptor != null;
        }
    }
}
