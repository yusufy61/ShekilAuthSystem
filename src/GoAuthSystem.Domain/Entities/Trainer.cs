namespace GoAuthSystem.Domain.Entities;

/// <summary>
/// Eğitmen entity'si — demo amaçlı basit entity.
/// İleride GoAthleticWebApplicationMVC projesine taşınacak.
/// </summary>
public class Trainer
{
    /// <summary>
    /// Birincil anahtar
    /// </summary>
    public int PersonId { get; set; }

    /// <summary>
    /// Eğitmenin adı
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Eğitmenin soyadı
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// E-posta adresi
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Telefon numarası
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Uzmanlık alanı
    /// </summary>
    public string Speciality { get; set; } = string.Empty;

    // ── Navigation Property ──────────────────────

    /// <summary>
    /// Eğitmenin kullanıcı hesabı (opsiyonel)
    /// </summary>
    public AppUser? AppUser { get; set; }
}
