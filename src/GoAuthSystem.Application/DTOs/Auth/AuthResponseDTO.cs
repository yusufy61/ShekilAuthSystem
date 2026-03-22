using GoAuthSystem.Application.DTOs.User;

namespace GoAuthSystem.Application.DTOs.Auth;

/// <summary>
/// Kimlik doğrulama yanıt DTO'su.
/// Login ve Register işlemlerinden dönen ortak yanıt yapısı.
/// </summary>
public class AuthResponseDTO
{
    /// <summary>
    /// JWT access token — API isteklerinde Authorization header'ında gönderilir
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token — access token süresi dolduğunda yenisini almak için kullanılır
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Access token'ın son kullanma tarihi (UTC)
    /// </summary>
    public DateTime Expiration { get; set; }

    /// <summary>
    /// Giriş yapan kullanıcının özet bilgileri
    /// </summary>
    public UserInfoDTO? UserInfo { get; set; }
}
