using GoAuthSystem.Domain.Interfaces;
using GoAuthSystem.Persistence.Data;
using GoAuthSystem.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoAuthSystem.Persistence;

/// <summary>
/// Persistence katmanı DI kayıtları.
/// DbContext, Repository ve UnitOfWork servislerini kaydeder.
/// Program.cs'den çağrılır: services.AddPersistenceServices(configuration);
/// </summary>
public static class ServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext — SQL Server bağlantısı
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
            )
        );

        // Generic Repository — tüm entity tipleri için açık generic kayıt
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
