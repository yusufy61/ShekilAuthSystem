using Microsoft.AspNetCore.Identity;

namespace GoAuthSystem.Domain.Entities;

/// <summary>
/// Uygulama kullanıcı entity'si.
/// IdentityUser<int> türetilmiş — PK tipi int.
/// </summary>
public class AppUser : IdentityUser<int>
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
    /// Tam ad (hesaplanmış özellik — veritabanına yazılmaz)
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Profil fotoğrafı URL'i (opsiyonel)
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Adres bilgisi (opsiyonel)
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Hesap oluşturulma tarihi (UTC)
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Hesap aktif/pasif durumu
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Eğitmen FK (nullable) — Kullanıcı bir eğitmense bu alan dolu olur
    /// </summary>
    public int? TrainerId { get; set; }

    /// <summary>
    /// Öğrenci FK (nullable) — Kullanıcı bir öğrenciyse bu alan dolu olur
    /// </summary>
    public int? TraineeId { get; set; }

    // ── Navigation Properties ──────────────────────

    /// <summary>
    /// Kullanıcıya ait refresh token'lar (1-N ilişki)
    /// </summary>
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
