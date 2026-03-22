using GoAuthSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoAuthSystem.Persistence.Configurations;

/// <summary>
/// AppUser entity konfigürasyonu.
/// Identity tarafından oluşturulan tabloya ek alanların ayarlarını yapar.
/// </summary>
public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        // Ad — zorunlu, max 50 karakter
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        // Soyad — zorunlu, max 50 karakter
        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        // FullName computed property → veritabanına yazılmaz
        builder.Ignore(u => u.FullName);

        // Profil resmi URL — opsiyonel, max 500 karakter
        builder.Property(u => u.ImageUrl)
            .HasMaxLength(500);

        // Adres — opsiyonel, max 250 karakter
        builder.Property(u => u.Address)
            .HasMaxLength(250);

        // Oluşturulma tarihi — varsayılan UTC now
        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Aktiflik durumu — varsayılan true
        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        // Trainer FK (1-1 ilişki, opsiyonel)
        builder.HasOne<Trainer>()
            .WithOne(t => t.AppUser)
            .HasForeignKey<AppUser>(u => u.TrainerId)
            .OnDelete(DeleteBehavior.SetNull);

        // Trainee FK (1-1 ilişki, opsiyonel)
        builder.HasOne<Trainee>()
            .WithOne(t => t.AppUser)
            .HasForeignKey<AppUser>(u => u.TraineeId)
            .OnDelete(DeleteBehavior.SetNull);

        // RefreshTokens ilişkisi (1-N)
        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.AppUser)
            .HasForeignKey(rt => rt.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
