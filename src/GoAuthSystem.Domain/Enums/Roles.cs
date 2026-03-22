namespace GoAuthSystem.Domain.Enums;

/// <summary>
/// Uygulama rolleri.
/// Seed data oluştururken ve rol kontrollerinde bu enum kullanılır.
/// </summary>
public enum Roles
{
    /// <summary>
    /// Yönetici — tüm yetkilere sahip
    /// </summary>
    Admin,

    /// <summary>
    /// Eğitmen — öğrenci yönetimi ve antrenman planı oluşturma
    /// </summary>
    Trainer,

    /// <summary>
    /// Öğrenci — kendi antrenman planını görüntüleme
    /// </summary>
    Student
}
