using GoAuthSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GoAuthSystem.Persistence.Data;

/// <summary>
/// Uygulama veritabanı bağlamı.
/// IdentityDbContext türevi — Identity tablolarını otomatik yönetir.
/// PK tipi int olarak ayarlanmış.
/// Seed Data: HasData yerine runtime seeding kullanılır (DbInitializer).
/// </summary>
public class AppDbContext : IdentityDbContext<AppUser, AppRole, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // ── DbSet Tanımları ──────────────────────────────
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Trainer> Trainers => Set<Trainer>();
    public DbSet<Trainee> Trainees => Set<Trainee>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Identity tablolarını oluştur (zorunlu çağrı)
        base.OnModelCreating(builder);

        // Configurations klasöründeki tüm IEntityTypeConfiguration'ları uygula
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // NOT: Seed data Runtime'da yapılır (DbInitializer.SeedAsync)
        // HasData yerine RoleManager/UserManager kullanılır — Identity için daha güvenli
    }
}
