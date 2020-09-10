using Microsoft.Extensions.Configuration;

namespace AddinEngine.Abstract
{
    public interface IConfigurationRegister
    {
        IConfigurationBuilder AddJsonFile(string path, bool optional);
    }
}
