using System.Security.Claims;
using GoAuthSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GoAuthSystem.Infrastructure.Services;

/// <summary>
/// Mevcut oturum açmış kullanıcı bilgilerini sağlayan servis.
/// IHttpContextAccessor üzerinden ClaimsPrincipal'a erişir.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Claims'deki kullanıcı referansı
    /// </summary>
    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    /// <summary>
    /// Mevcut kullanıcının ID'si
    /// </summary>
    public int? UserId
    {
        get
        {
            var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var id) ? id : null;
        }
    }

    /// <summary>
    /// Mevcut kullanıcının e-posta adresi
    /// </summary>
    public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value;

    /// <summary>
    /// Mevcut kullanıcının rolü
    /// </summary>
    public string? Role => User?.FindFirst(ClaimTypes.Role)?.Value;

    /// <summary>
    /// Kullanıcı oturum açmış mı?
    /// </summary>
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}
