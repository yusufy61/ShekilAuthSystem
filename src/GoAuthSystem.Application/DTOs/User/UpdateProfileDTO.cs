namespace GoAuthSystem.Application.DTOs.User;

/// <summary>
/// Profil güncelleme DTO'su.
/// Kullanıcı kendi profil bilgilerini güncellerken kullanılır.
/// </summary>
public class UpdateProfileDTO
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
    /// Adres bilgisi (opsiyonel)
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Profil fotoğrafı URL'i (opsiyonel)
    /// </summary>
    public string? ImageUrl { get; set; }
}
