using GoAuthSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoAuthSystem.Persistence.Configurations;

/// <summary>
/// Trainer entity konfigürasyonu.
/// </summary>
public class TrainerConfiguration : IEntityTypeConfiguration<Trainer>
{
    public void Configure(EntityTypeBuilder<Trainer> builder)
    {
        builder.ToTable("Trainers");

        // PK
        builder.HasKey(t => t.PersonId);

        builder.Property(t => t.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(t => t.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(t => t.Speciality)
            .HasMaxLength(100);
    }
}
