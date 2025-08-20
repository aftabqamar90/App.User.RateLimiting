using Microsoft.AspNetCore.Mvc;

namespace App.User.RateLimiting.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VersionController : ControllerBase
{
    [HttpGet]
    public IActionResult GetVersions()
    {
        return Ok(new
        {
            currentVersion = "2.0",
            supportedVersions = new[] { "1.0", "2.0" },
            deprecatedVersions = new[] { "1.0" },
            latestVersion = "2.0",
            message = "Use the latest version for the best experience"
        });
    }

    [HttpGet("v1")]
    public IActionResult GetV1Info()
    {
        return Ok(new
        {
            version = "1.0",
            status = "deprecated",
            message = "This version is deprecated. Please upgrade to v2.0",
            sunsetDate = "2024-12-31",
            endpoints = new[] { "GET /api/v1/publicapi", "POST /api/v1/publicapi" }
        });
    }

    [HttpGet("v2")]
    public IActionResult GetV2Info()
    {
        return Ok(new
        {
            version = "2.0",
            status = "current",
            message = "This is the current version with enhanced features",
            endpoints = new[] { "GET /api/v2/publicapi", "POST /api/v2/publicapi", "GET /api/v2/publicapi/status" },
            features = new[] { "Enhanced logging", "Version info", "Status endpoint", "Feature flags" }
        });
    }
}
