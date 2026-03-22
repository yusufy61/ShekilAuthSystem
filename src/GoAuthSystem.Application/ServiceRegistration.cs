using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace GoAuthSystem.Application;

/// <summary>
/// Application katmanı servis kayıtları.
/// AutoMapper profilleri ve FluentValidation validator'ları DI container'a kaydeder.
/// Program.cs'den çağrılır: services.AddApplicationServices();
/// </summary>
public static class ServiceRegistration
{
    /// <summary>
    /// Application katmanı servislerini DI container'a kaydet.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // AutoMapper — bu assembly'deki tüm Profile sınıflarını otomatik bul ve kaydet
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // FluentValidation — bu assembly'deki tüm AbstractValidator<T> sınıflarını otomatik bul ve kaydet
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
