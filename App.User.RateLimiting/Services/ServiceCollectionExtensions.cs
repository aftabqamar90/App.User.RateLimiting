using Microsoft.Extensions.DependencyInjection;

namespace App.User.RateLimiting.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRateLimitServices(this IServiceCollection services)
        {
            services.AddScoped<IRateLimitConfigurationService, RateLimitConfigurationService>();
            return services;
        }

        public static IServiceCollection AddConfigurationServices(this IServiceCollection services)
        {
            services.AddScoped<IConfigurationService, ConfigurationService>();
            return services;
        }

        public static IServiceCollection AddEmptyServices(this IServiceCollection services)
        {
            services.AddScoped<IEmptyService, EmptyService>();
            return services;
        }

        public static IServiceCollection AddAllServices(this IServiceCollection services)
        {
            services.AddRateLimitServices()
                   .AddConfigurationServices()
                   .AddEmptyServices();
            return services;
        }
    }
}
