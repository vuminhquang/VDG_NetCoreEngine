using AddinEngine.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AddinEngine
{
    public class ConfigurationRegister : IConfigurationRegister
    {
        private readonly IConfigurationBuilder _configs;

        public ConfigurationRegister(IConfigurationBuilder configs)
        {
            _configs = configs;
        }

        public IConfigurationBuilder AddJsonFile(string path, bool optional)
        {
            return _configs.AddJsonFile(path, optional);
        }
    }
}
