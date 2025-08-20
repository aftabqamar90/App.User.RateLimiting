using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.RateLimiting;
using Asp.Versioning;

namespace App.User.RateLimiting.Controllers;

[ApiController]
[ApiVersion("2.0")]
[Route("api/publicapi")]
[EnableCors("PublicApiCors")]
[EnableRateLimiting("DynamicUserRateLimit")]
public class PublicApiV2Controller : ControllerBase
{
    private readonly ILogger<PublicApiV2Controller> _logger;

    public PublicApiV2Controller(ILogger<PublicApiV2Controller> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var userId = GetUserIdFromRequest();
        var timestamp = DateTime.UtcNow;
        var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "2.0";

        _logger.LogInformation("API v2 Request - User: {UserId}, Endpoint: {Endpoint}, Timestamp: {Timestamp}, IP: {IP}, Version: {Version}",
            userId, "GET /api/v2/publicapi", timestamp, HttpContext.Connection.RemoteIpAddress, version);

        return Ok(new
        {
            message = "Public API v2 endpoint",
            timestamp,
            userId,
            version,
            features = new[] { "Enhanced logging", "Version info", "Feature flags" }
        });
    }

    [HttpPost]
    public IActionResult Post([FromBody] object data)
    {
        var userId = GetUserIdFromRequest();
        var timestamp = DateTime.UtcNow;
        var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "2.0";

        _logger.LogInformation("API v2 Request - User: {UserId}, Endpoint: {Endpoint}, Timestamp: {Timestamp}, IP: {IP}, DataSize: {DataSize}, Version: {Version}",
            userId, "POST /api/v2/publicapi", timestamp, HttpContext.Connection.RemoteIpAddress,
            System.Text.Json.JsonSerializer.Serialize(data).Length, version);

        return Ok(new
        {
            message = "Data received via v2 API",
            timestamp,
            userId,
            version,
            processingTime = "Enhanced v2 processing"
        });
    }

    [HttpGet("status")]
    [MapToApiVersion("2.0")]
    public IActionResult GetStatus()
    {
        var userId = GetUserIdFromRequest();
        var timestamp = DateTime.UtcNow;

        return Ok(new
        {
            status = "healthy",
            timestamp,
            userId,
            version = "2.0",
            uptime = DateTime.UtcNow - DateTime.UnixEpoch
        });
    }

    private string GetUserIdFromRequest()
    {
        return Request.Query["userId"].FirstOrDefault() ??
               Request.Headers["X-User-Id"].FirstOrDefault() ??
               "anonymous";
    }
}
