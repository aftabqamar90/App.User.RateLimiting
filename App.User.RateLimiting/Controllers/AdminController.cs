using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;

namespace App.User.RateLimiting.Controllers;

[ApiController]
[Route("admin/[controller]")]
[DisableCors]
public class AdminController : ControllerBase
{
    private readonly ILogger<AdminController> _logger;

    public AdminController(ILogger<AdminController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var user = "Test-Admin-User";
        var timestamp = DateTime.UtcNow;
        
        _logger.LogInformation("Admin Request - User: {User}, Endpoint: {Endpoint}, Timestamp: {Timestamp}", 
            user, "GET /admin/admin", timestamp);
        
        return Ok(new { message = "Admin endpoint", timestamp, user });
    }

    [HttpPost]
    public IActionResult Post([FromBody] object data)
    {
        var user = "Test-Admin-User";
        var timestamp = DateTime.UtcNow;
        
        _logger.LogInformation("Admin Request - User: {User}, Endpoint: {Endpoint}, Timestamp: {Timestamp}", 
            user, "POST /admin/admin", timestamp);
        
        return Ok(new { message = "Admin data received", timestamp, user });
    }

    [HttpGet("users")]
    public IActionResult GetUsers()
    {
        var user = "Test-Admin-User";
        var timestamp = DateTime.UtcNow;
        
        _logger.LogInformation("Admin Request - User: {User}, Endpoint: {Endpoint}, Timestamp: {Timestamp}", 
            user, "GET /admin/admin/users", timestamp);
        
        return Ok(new { message = "Users list", timestamp, user, users = new[] { "user1", "user2", "user3" } });
    }
}
