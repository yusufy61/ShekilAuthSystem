using GoAuthSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoAuthSystem.Persistence.Configurations;

/// <summary>
/// RefreshToken entity konfigürasyonu.
/// Token tablosunun yapısını ve index'lerini tanımlar.
/// </summary>
public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        // Tablo adı
        builder.ToTable("RefreshTokens");

        // PK
        builder.HasKey(rt => rt.Id);

        // Token — zorunlu, max 256 karakter
        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(256);

        // Oluşturulma tarihi — zorunlu
        builder.Property(rt => rt.Created)
            .IsRequired();

        // Son kullanma tarihi — zorunlu
        builder.Property(rt => rt.Expires)
            .IsRequired();

        // Yerine geçen token — opsiyonel, max 256 karakter
        builder.Property(rt => rt.ReplacedByToken)
            .HasMaxLength(256);

        // İptal sebebi — opsiyonel, max 256 karakter
        builder.Property(rt => rt.ReasonRevoked)
            .HasMaxLength(256);

        // Computed property'leri veritabanına yazma
        builder.Ignore(rt => rt.IsExpired);
        builder.Ignore(rt => rt.IsRevoked);
        builder.Ignore(rt => rt.IsActive);

        // Token üzerinde index — hızlı arama için
        builder.HasIndex(rt => rt.Token);

        // AppUserId üzerinde index — kullanıcının tokenlarını hızlı bulmak için
        builder.HasIndex(rt => rt.AppUserId);
    }
}
