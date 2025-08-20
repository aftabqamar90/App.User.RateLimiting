using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace App.User.RateLimiting.Services
{
    public interface IRateLimitConfigurationService
    {
        void ConfigureRateLimiting(RateLimiterOptions options);
        int GetUserRateLimit(string userId);
    }

    public class RateLimitConfigurationService : IRateLimitConfigurationService
    {
        private readonly IConfiguration _configuration;

        public RateLimitConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureRateLimiting(RateLimiterOptions options)
        {
            // Add a policy that partitions by user and applies different limits
            options.AddPolicy("DynamicUserRateLimit", httpContext =>
            {
                var userId = httpContext.Request.Query["userId"].FirstOrDefault() ??
                            httpContext.Request.Headers["X-User-Id"].FirstOrDefault() ??
                            "anonymous";

                var permitLimit = GetUserRateLimit(userId);

                return RateLimitPartition.GetFixedWindowLimiter(userId, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = permitLimit * 100,
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });
            });
        }

        public int GetUserRateLimit(string userId)
        {
            return userId switch
            {
                "anonymous" => 1,   // Anonymous users: 1 request per minute
                "user1" => 1,       // Free tier: 1 request per minute
                "user2" => 20,      // Basic tier: 20 requests per minute
                "user3" => 100,     // Professional tier: 100 requests per minute
                "user4" => 500,     // Enterprise tier: 500 requests per minute
                "admin" => 1000,    // Admin: 1000 requests per minute
                "vip" => 2000,      // VIP: 2000 requests per minute
                _ => 1              // Default to 1 request per minute for unknown users
            };
        }
    }
}
