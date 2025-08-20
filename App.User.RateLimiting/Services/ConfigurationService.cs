using Microsoft.Extensions.Configuration;

namespace App.User.RateLimiting.Services
{
    public interface IConfigurationService
    {
        // Interface methods can be added here as needed
    }

    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfiguration _configuration;

        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Implementation methods can be added here as needed
    }
}
