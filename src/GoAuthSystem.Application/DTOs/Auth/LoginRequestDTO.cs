namespace GoAuthSystem.Application.DTOs.Auth;

/// <summary>
/// Giriş isteği DTO'su.
/// Kullanıcı e-posta ve şifre ile giriş yapar.
/// </summary>
public class LoginRequestDTO
{
    /// <summary>
    /// Kullanıcı e-posta adresi
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Kullanıcı şifresi
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
