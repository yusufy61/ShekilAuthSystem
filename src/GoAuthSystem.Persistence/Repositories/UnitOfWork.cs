using GoAuthSystem.Domain.Interfaces;
using GoAuthSystem.Persistence.Data;

namespace GoAuthSystem.Persistence.Repositories;

/// <summary>
/// Unit of Work implementasyonu.
/// Birden fazla repository'nin tek bir transaction içinde çalışmasını sağlar.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Bekleyen tüm değişiklikleri veritabanına yaz
    /// </summary>
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// DbContext'i dispose et
    /// </summary>
    public void Dispose()
    {
        _context.Dispose();
    }
}
