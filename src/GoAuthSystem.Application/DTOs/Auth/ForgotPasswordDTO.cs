namespace GoAuthSystem.Application.DTOs.Auth;

/// <summary>
/// Şifremi unuttum DTO'su.
/// Kullanıcı e-posta adresini gönderir, sistem şifre sıfırlama bağlantısı gönderir.
/// </summary>
public class ForgotPasswordDTO
{
    /// <summary>
    /// Şifre sıfırlama bağlantısı gönderilecek e-posta adresi
    /// </summary>
    public string Email { get; set; } = string.Empty;
}
