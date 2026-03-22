namespace GoAuthSystem.Domain.Interfaces;

/// <summary>
/// Unit of Work arayüzü.
/// Birden fazla repository'nin tek bir transaction içinde çalışmasını sağlar.
/// SaveChangesAsync çağrılana kadar değişiklikler veritabanına yazılmaz.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Bekleyen tüm değişiklikleri veritabanına yaz
    /// </summary>
    /// <returns>Etkilenen kayıt sayısı</returns>
    Task<int> SaveChangesAsync();
}
