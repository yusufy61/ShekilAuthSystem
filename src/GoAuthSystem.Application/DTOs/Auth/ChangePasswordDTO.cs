namespace GoAuthSystem.Application.DTOs.Auth;

/// <summary>
/// Şifre değiştirme DTO'su.
/// Oturum açmış kullanıcı mevcut şifresini doğrulayarak yeni şifre belirler.
/// </summary>
public class ChangePasswordDTO
{
    /// <summary>
    /// Mevcut şifre — doğrulama için gerekli
    /// </summary>
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// Yeni şifre — güçlü şifre kurallarına uymalı
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Yeni şifre tekrarı — NewPassword ile eşleşmeli
    /// </summary>
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
