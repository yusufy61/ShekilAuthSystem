namespace GoAuthSystem.Domain.Entities;

/// <summary>
/// Refresh Token entity'si — AppUser ile 1-N ilişki.
/// Her kullanıcının birden fazla refresh token'ı olabilir (rotation zinciri).
/// Ayrı bir tabloda saklanır.
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// Birincil anahtar
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Token değeri — 64 byte, base64 encoded, rastgele üretilir
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Token oluşturulma tarihi (UTC)
    /// </summary>
    public DateTime Created { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Token son kullanma tarihi (UTC)
    /// </summary>
    public DateTime Expires { get; set; }

    /// <summary>
    /// Token iptal tarihi (UTC) — null ise henüz iptal edilmemiş
    /// </summary>
    public DateTime? Revoked { get; set; }

    /// <summary>
    /// Rotation sırasında bu token'ın yerine geçen yeni token değeri.
    /// Token rotation zincirini takip etmeye yarar.
    /// </summary>
    public string? ReplacedByToken { get; set; }

    /// <summary>
    /// Token'ın iptal edilme sebebi (opsiyonel)
    /// </summary>
    public string? ReasonRevoked { get; set; }

    // ── Computed Properties (veritabanına yazılmaz) ──────

    /// <summary>
    /// Token süresi dolmuş mu?
    /// </summary>
    public bool IsExpired => DateTime.UtcNow >= Expires;

    /// <summary>
    /// Token iptal edilmiş mi?
    /// </summary>
    public bool IsRevoked => Revoked != null;

    /// <summary>
    /// Token aktif mi? (ne iptal edilmiş ne de süresi dolmuş)
    /// </summary>
    public bool IsActive => !IsRevoked && !IsExpired;

    // ── Foreign Key & Navigation ──────────────────────

    /// <summary>
    /// Kullanıcı FK
    /// </summary>
    public int AppUserId { get; set; }

    /// <summary>
    /// Kullanıcı navigation property
    /// </summary>
    public AppUser AppUser { get; set; } = null!;
}
