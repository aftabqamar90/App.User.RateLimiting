using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.RateLimiting;
using Asp.Versioning;

namespace App.User.RateLimiting.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/publicapi")]
[EnableCors("PublicApiCors")]
[EnableRateLimiting("DynamicUserRateLimit")]
public class PublicApiController : ControllerBase
{
    private readonly ILogger<PublicApiController> _logger;

    public PublicApiController(ILogger<PublicApiController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var userId = GetUserIdFromRequest();
        var timestamp = DateTime.UtcNow;
        var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";

        _logger.LogInformation("API v1 Request - User: {UserId}, Endpoint: {Endpoint}, Timestamp: {Timestamp}, IP: {IP}, Version: {Version}",
            userId, "GET /api/v1/publicapi", timestamp, HttpContext.Connection.RemoteIpAddress, version);

        return Ok(new
        {
            message = "Public API v1 endpoint",
            timestamp,
            userId,
            version
        });
    }

    [HttpPost]
    public IActionResult Post([FromBody] object data)
    {
        var userId = GetUserIdFromRequest();
        var timestamp = DateTime.UtcNow;
        var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";

        _logger.LogInformation("API v1 Request - User: {UserId}, Endpoint: {Endpoint}, Timestamp: {Timestamp}, IP: {IP}, DataSize: {DataSize}, Version: {Version}",
            userId, "POST /api/v1/publicapi", timestamp, HttpContext.Connection.RemoteIpAddress,
            System.Text.Json.JsonSerializer.Serialize(data).Length, version);

        return Ok(new
        {
            message = "Data received via v1 API",
            timestamp,
            userId,
            version
        });
    }



    private string GetUserIdFromRequest()
    {
        return Request.Query["userId"].FirstOrDefault() ??
               Request.Headers["X-User-Id"].FirstOrDefault() ??
               "anonymous";
    }
}
