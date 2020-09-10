using System;
using Microsoft.Extensions.Configuration;

namespace AddinEngine.Abstract
{
    public interface ICommandProvider
    {
        ISyncCommand CreateCommand(string type);
        Type[] GetAvailableCommands();
        IServiceProvider ServicesProvider { get; }
        IConfiguration Configuration { get; }
    }
}