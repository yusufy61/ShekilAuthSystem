using GoAuthSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoAuthSystem.Persistence.Configurations;

/// <summary>
/// Trainee entity konfigürasyonu.
/// </summary>
public class TraineeConfiguration : IEntityTypeConfiguration<Trainee>
{
    public void Configure(EntityTypeBuilder<Trainee> builder)
    {
        builder.ToTable("Trainees");

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

        // Trainer FK (N-1 ilişki)
        builder.HasOne(t => t.Trainer)
            .WithMany()
            .HasForeignKey(t => t.TrainerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
