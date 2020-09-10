using System;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace AddinEngine.Abstract
{
    public abstract class CommandProviderBase : ICommandProvider
    {
        protected readonly IServiceProvider _services;
        protected readonly IConfiguration _configuration;
        public IServiceProvider ServicesProvider => _services;
        public IConfiguration Configuration => _configuration;
        
        private CommandProviderBase(){}

        protected CommandProviderBase(IServiceProvider services, IConfiguration configuration)
        {
            _services = services;
            _configuration = configuration;
        }

        public ISyncCommand CreateCommand(string commandType)
        {
            return CreateCommand(this.GetType().Assembly, commandType);
        }

        protected ISyncCommand CreateCommand(Assembly storingAssembly, string commandType)
        {
            return (
                from exportedType in storingAssembly.GetExportedTypes()
                where exportedType.FullName == commandType
                select Activator.CreateInstance(exportedType, this, _services) as ISyncCommand
            ).FirstOrDefault();
        }

        public Type[] GetAvailableCommands()
        {
            var storingAssembly = GetType().Assembly;
            return (from exportedType in storingAssembly.GetExportedTypes()
                    .Where(y => typeof(ISyncCommand).IsAssignableFrom(y) && !y.IsInterface && !y.IsAbstract)
                select exportedType).ToArray();
        }
    }
}