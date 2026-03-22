using System.Linq.Expressions;
using GoAuthSystem.Domain.Interfaces;
using GoAuthSystem.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace GoAuthSystem.Persistence.Repositories;

/// <summary>
/// Generic repository implementasyonu.
/// Tüm entity'ler için temel CRUD işlemlerini sağlar.
/// </summary>
/// <typeparam name="T">Entity tipi</typeparam>
public class GenericRepository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    /// <summary>
    /// ID'ye göre entity getir
    /// </summary>
    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <summary>
    /// Tüm entity'leri getir
    /// </summary>
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    /// <summary>
    /// Koşula göre entity'leri filtrele
    /// </summary>
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).AsNoTracking().ToListAsync();
    }

    /// <summary>
    /// Yeni entity ekle
    /// </summary>
    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    /// <summary>
    /// Entity güncelle
    /// </summary>
    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    /// <summary>
    /// Entity sil
    /// </summary>
    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
}
