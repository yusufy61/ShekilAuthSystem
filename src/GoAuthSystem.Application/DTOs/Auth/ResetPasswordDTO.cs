namespace GoAuthSystem.Application.DTOs.Auth;

/// <summary>
/// Şifre sıfırlama DTO'su.
/// E-posta ile gelen sıfırlama token'ı ve yeni şifre bilgisi.
/// </summary>
public class ResetPasswordDTO
{
    /// <summary>
    /// Kullanıcı e-posta adresi
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Şifre sıfırlama token'ı (e-posta ile gönderilen)
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Yeni şifre
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Yeni şifre tekrarı — NewPassword ile eşleşmeli
    /// </summary>
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
