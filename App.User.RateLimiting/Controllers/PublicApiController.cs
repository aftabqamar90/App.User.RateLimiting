using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.RateLimiting;

namespace App.User.RateLimiting.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("PublicApiCors")]
[EnableRateLimiting("ApiKeyRateLimit")]
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
        var apiKey = "Test-API-Key";
        var timestamp = DateTime.UtcNow;
        
        _logger.LogInformation("API Request - Key: {ApiKey}, Endpoint: {Endpoint}, Timestamp: {Timestamp}, IP: {IP}", 
            apiKey, "GET /api/publicapi", timestamp, HttpContext.Connection.RemoteIpAddress);
        
        return Ok(new { message = "Public API endpoint", timestamp, apiKey });
    }

    [HttpPost]
    public IActionResult Post([FromBody] object data)
    {
        var apiKey = "Test-API-Key";
        var timestamp = DateTime.UtcNow;
        
        _logger.LogInformation("API Request - Key: {ApiKey}, Endpoint: {Endpoint}, Timestamp: {Timestamp}, IP: {IP}, DataSize: {DataSize}", 
            apiKey, "Test-API-Key", timestamp, HttpContext.Connection.RemoteIpAddress, 
            System.Text.Json.JsonSerializer.Serialize(data).Length);
        
        return Ok(new { message = "Data received", timestamp, apiKey });
    }
}
