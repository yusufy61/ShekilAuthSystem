namespace GoAuthSystem.Application.DTOs.User;

/// <summary>
/// Kullanıcı bilgileri DTO'su.
/// Login/Register yanıtında ve profil görüntülemede kullanılır.
/// </summary>
public class UserInfoDTO
{
    /// <summary>
    /// Kullanıcı ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Tam ad (Ad + Soyad)
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// E-posta adresi
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Kullanıcının rolü (Admin, Trainer, Student)
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Profil fotoğrafı URL'i
    /// </summary>
    public string? ImageUrl { get; set; }
}
