using GoAuthSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoAuthSystem.Persistence.Seeds;

/// <summary>
/// Rol seed data — Admin, Trainer, Student rollerini oluşturur.
/// Migration sırasında otomatik olarak veritabanına yazılır.
/// </summary>
public static class RoleSeedData
{
    public static void Seed(ModelBuilder builder)
    {
        builder.Entity<AppRole>().HasData(
            new AppRole
            {
                Id = 1,
                Name = "Admin",
                NormalizedName = "ADMIN",
                Description = "Yönetici — tüm yetkilere sahip",
                ConcurrencyStamp = "1"
            },
            new AppRole
            {
                Id = 2,
                Name = "Trainer",
                NormalizedName = "TRAINER",
                Description = "Eğitmen — öğrenci yönetimi ve antrenman planı oluşturma",
                ConcurrencyStamp = "2"
            },
            new AppRole
            {
                Id = 3,
                Name = "Student",
                NormalizedName = "STUDENT",
                Description = "Öğrenci — kendi antrenman planını görüntüleme",
                ConcurrencyStamp = "3"
            }
        );
    }
}
