namespace GoAuthSystem.Domain.Entities;

/// <summary>
/// Öğrenci entity'si — demo amaçlı basit entity.
/// İleride GoAthleticWebApplicationMVC projesine taşınacak.
/// </summary>
public class Trainee
{
    /// <summary>
    /// Birincil anahtar
    /// </summary>
    public int PersonId { get; set; }

    /// <summary>
    /// Öğrencinin adı
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Öğrencinin soyadı
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

    // ── Foreign Key & Navigation ──────────────────────

    /// <summary>
    /// Eğitmen FK (nullable) — Hangi eğitmenin öğrencisi
    /// </summary>
    public int? TrainerId { get; set; }

    /// <summary>
    /// Eğitmen navigation property
    /// </summary>
    public Trainer? Trainer { get; set; }

    /// <summary>
    /// Öğrencinin kullanıcı hesabı (opsiyonel)
    /// </summary>
    public AppUser? AppUser { get; set; }
}
