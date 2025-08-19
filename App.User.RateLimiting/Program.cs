using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using App.User.RateLimiting.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "ApiKey";
    options.DefaultChallengeScheme = "ApiKey";
})
.AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKey", options => { })
.AddJwtBearer("AzureAD", options =>
{
    options.Authority = builder.Configuration["AzureAd:Instance"] + builder.Configuration["AzureAd:TenantId"];
    options.Audience = builder.Configuration["AzureAd:ClientId"];
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("PublicApiCors", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});



builder.Services.AddRateLimiter(options =>
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
            PermitLimit = permitLimit,
            Window = TimeSpan.FromMinutes(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("App.User.RateLimiting")
        .WithTheme(ScalarTheme.Laserwave)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();


app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"), appBuilder =>
{
    appBuilder.UseRateLimiter();
});

app.UseWhen(context => context.Request.Path.StartsWithSegments("/admin"), appBuilder =>
{
    app.UseCors("PublicApiCors");
});

app.MapControllers();

app.Run();

// Helper function to get user-specific rate limits
static int GetUserRateLimit(string userId)
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
