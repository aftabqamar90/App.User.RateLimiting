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
    options.AddFixedWindowLimiter("ApiKeyRateLimit", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;          // Only 1 request allowed
        limiterOptions.Window = TimeSpan.FromMinutes(1); // Per minute
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;          // No queuing allowed
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
    appBuilder.UseCors("PublicApiCors");
    appBuilder.UseRateLimiter();
});

app.UseWhen(context => context.Request.Path.StartsWithSegments("/admin"), appBuilder =>
{
    // Admin-specific middleware can be added here
});

app.MapControllers();

app.Run();
