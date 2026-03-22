using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoAuthSystem.API.Controllers;

/// <summary>
/// Yetkilendirme test controller'ı.
/// Her rol için ayrı endpoint — Swagger'dan kolayca test edilebilir.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    /// <summary>
    /// Herkese açık endpoint — token gerektirmez.
    /// </summary>
    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult Public()
    {
        return Ok(new
        {
            message = "Bu endpoint herkese açıktır.",
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Oturum açmış kullanıcı — herhangi bir rol yeterli.
    /// </summary>
    [HttpGet("authenticated")]
    [Authorize]
    public IActionResult Authenticated()
    {
        return Ok(new
        {
            message = "Bu endpoint'e sadece oturum açmış kullanıcılar erişebilir.",
            user = User.Identity?.Name,
            claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }

    /// <summary>
    /// Sadece Admin rolü erişebilir.
    /// </summary>
    [HttpGet("admin-only")]
    [Authorize(Roles = "Admin")]
    public IActionResult AdminOnly()
    {
        return Ok(new
        {
            message = "Bu endpoint'e sadece Admin rolüne sahip kullanıcılar erişebilir.",
            role = "Admin"
        });
    }

    /// <summary>
    /// Sadece Trainer rolü erişebilir.
    /// </summary>
    [HttpGet("trainer-only")]
    [Authorize(Roles = "Trainer")]
    public IActionResult TrainerOnly()
    {
        return Ok(new
        {
            message = "Bu endpoint'e sadece Trainer rolüne sahip kullanıcılar erişebilir.",
            role = "Trainer"
        });
    }

    /// <summary>
    /// Sadece Student rolü erişebilir.
    /// </summary>
    [HttpGet("student-only")]
    [Authorize(Roles = "Student")]
    public IActionResult StudentOnly()
    {
        return Ok(new
        {
            message = "Bu endpoint'e sadece Student rolüne sahip kullanıcılar erişebilir.",
            role = "Student"
        });
    }

    /// <summary>
    /// Admin VEYA Trainer rolü erişebilir.
    /// </summary>
    [HttpGet("trainer-or-admin")]
    [Authorize(Roles = "Admin,Trainer")]
    public IActionResult TrainerOrAdmin()
    {
        return Ok(new
        {
            message = "Bu endpoint'e Admin veya Trainer rolüne sahip kullanıcılar erişebilir.",
            currentRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value
        });
    }
}
