namespace GoAuthSystem.Application.DTOs.Auth;

/// <summary>
/// Refresh token yenileme isteği DTO'su.
/// Süresi dolmuş access token ve geçerli refresh token gönderilir.
/// </summary>
public class RefreshTokenRequestDTO
{
    /// <summary>
    /// Süresi dolmuş access token — kullanıcı kimliğini belirlemek için kullanılır
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Geçerli refresh token — yeni token çifti almak için kullanılır
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}
