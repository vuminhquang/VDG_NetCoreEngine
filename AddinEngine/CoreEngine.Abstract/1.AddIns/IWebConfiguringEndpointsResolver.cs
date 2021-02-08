namespace AddinEngine.Abstract
{
    public interface IWebConfiguringEndpointsResolver
    {
        void ConfigureEndpoints(dynamic endpoints, dynamic env);
    }
}