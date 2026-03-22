using GoAuthSystem.Domain.Entities;
using GoAuthSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace GoAuthSystem.API.Data;

/// <summary>
/// Veritabanı başlangıç verilerini oluşturur (runtime seeding).
/// HasData yerine RoleManager/UserManager kullanarak Identity nesneleri oluşturur.
/// Bu yaklaşım Identity hash/stamp yönetimi için daha güvenlidir.
/// Program.cs'den çağrılır.
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Rolleri ve varsayılan admin kullanıcısını oluştur.
    /// Uygulama her başlatıldığında çalışır, var olanları atlar.
    /// </summary>
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppRole>>(); // generic log

        // ── 1. Rolleri Oluştur ──────────────────────────
        var roles = new[]
        {
            new { Name = Roles.Admin.ToString(), Description = "Tam yetkili sistem yöneticisi" },
            new { Name = Roles.Trainer.ToString(), Description = "Antrenör paneline erişim" },
            new { Name = Roles.Student.ToString(), Description = "Öğrenci paneline erişim" }
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name))
            {
                var result = await roleManager.CreateAsync(new AppRole
                {
                    Name = role.Name,
                    Description = role.Description
                });

                if (result.Succeeded)
                    logger.LogInformation("Rol oluşturuldu: {Role}", role.Name);
                else
                    logger.LogError("Rol oluşturulamadı: {Role} — {Errors}",
                        role.Name, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // ── 2. Varsayılan Admin Kullanıcısını Oluştur ──────
        const string adminEmail = "admin@goathletic.com";
        const string adminPassword = "Admin123!@#";

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin == null)
        {
            var adminUser = new AppUser
            {
                FirstName = "System",
                LastName = "Admin",
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, Roles.Admin.ToString());
                logger.LogInformation("Varsayılan admin kullanıcısı oluşturuldu: {Email}", adminEmail);
            }
            else
            {
                logger.LogError("Admin kullanıcısı oluşturulamadı: {Errors}",
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            logger.LogInformation("Admin kullanıcısı zaten mevcut: {Email}", adminEmail);
        }
    }
}
