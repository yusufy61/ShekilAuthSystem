using System.Linq.Expressions;

namespace GoAuthSystem.Domain.Interfaces;

/// <summary>
/// Generic repository arayüzü — tüm entity'ler için temel CRUD işlemleri.
/// Somut implementasyon Persistence katmanında yapılacak.
/// </summary>
/// <typeparam name="T">Entity tipi</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// ID'ye göre entity getir
    /// </summary>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Tüm entity'leri getir
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Koşula göre entity'leri filtrele
    /// </summary>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Yeni entity ekle
    /// </summary>
    Task AddAsync(T entity);

    /// <summary>
    /// Entity güncelle
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Entity sil
    /// </summary>
    void Delete(T entity);
}
