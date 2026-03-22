namespace GoAuthSystem.Application.DTOs.Auth;

/// <summary>
/// Kayıt isteği DTO'su.
/// Yeni kullanıcı oluşturmak için gerekli bilgiler.
/// </summary>
public class RegisterRequestDTO
{
    /// <summary>
    /// Kullanıcının adı
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Kullanıcının soyadı
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// E-posta adresi (benzersiz olmalı)
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Şifre (en az 8 karakter, güçlü şifre kuralları)
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Şifre tekrarı — Password ile eşleşmeli
    /// </summary>
    public string ConfirmPassword { get; set; } = string.Empty;
}
