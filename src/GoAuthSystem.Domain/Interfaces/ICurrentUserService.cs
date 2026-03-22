namespace GoAuthSystem.Domain.Interfaces;

/// <summary>
/// Mevcut oturum açmış kullanıcı bilgilerini sağlayan servis arayüzü.
/// HttpContext.User (ClaimsPrincipal) üzerinden kullanıcı bilgilerini okur.
/// Somut implementasyon Infrastructure veya API katmanında yapılacak.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Mevcut kullanıcının ID'si (Claims'den okunur)
    /// </summary>
    int? UserId { get; }

    /// <summary>
    /// Mevcut kullanıcının e-posta adresi
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Mevcut kullanıcının rolü
    /// </summary>
    string? Role { get; }

    /// <summary>
    /// Kullanıcı oturum açmış mı?
    /// </summary>
    bool IsAuthenticated { get; }
}
