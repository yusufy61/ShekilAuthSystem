using GoAuthSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GoAuthSystem.Persistence.Seeds;

/// <summary>
/// Admin kullanıcısı seed data.
/// Varsayılan admin hesabı: admin@goathletic.com / Admin123!@#
/// Migration sırasında otomatik olarak veritabanına yazılır.
/// </summary>
public static class AdminSeedData
{
    public static void Seed(ModelBuilder builder)
    {
        // Admin kullanıcısı oluştur
        var adminUser = new AppUser
        {
            Id = 1,
            FirstName = "System",
            LastName = "Admin",
            UserName = "admin@goathletic.com",
            NormalizedUserName = "ADMIN@GOATHLETIC.COM",
            Email = "admin@goathletic.com",
            NormalizedEmail = "ADMIN@GOATHLETIC.COM",
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            SecurityStamp = "STATIC-SECURITY-STAMP-FOR-SEED",
            ConcurrencyStamp = "STATIC-CONCURRENCY-STAMP-FOR-SEED"
        };

        // Şifreyi hash'le — PasswordHasher Identity'nin dahili hasher'ını kullanır
        var hasher = new PasswordHasher<AppUser>();
        adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123!@#");

        builder.Entity<AppUser>().HasData(adminUser);

        // Admin rolünü ata (AspNetUserRoles tablosu)
        builder.Entity<IdentityUserRole<int>>().HasData(
            new IdentityUserRole<int>
            {
                UserId = 1,  // Admin kullanıcı ID
                RoleId = 1   // Admin rol ID
            }
        );
    }
}
