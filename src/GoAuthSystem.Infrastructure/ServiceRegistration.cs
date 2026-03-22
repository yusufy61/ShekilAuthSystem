using System.Text;
using GoAuthSystem.Domain.Entities;
using GoAuthSystem.Domain.Interfaces;
using GoAuthSystem.Infrastructure.Services;
using GoAuthSystem.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using GoAuthSystem.Persistence.Data;

namespace GoAuthSystem.Infrastructure;

/// <summary>
/// Infrastructure katmanı DI kayıtları.
/// Identity + JWT Bearer + servis implementasyonları.
/// Program.cs'den çağrılır: services.AddInfrastructureServices(configuration);
/// </summary>
public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        // ── JWT Ayarları (Options Pattern) ──────────────────
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        // ── Password Hasher Ayarları (PBKDF2-SHA512, 600K iterasyon) ──
        services.Configure<PasswordHasherOptions>(options =>
        {
            options.IterationCount = 600_000;
        });

        // ── ASP.NET Core Identity Konfigürasyonu ──────────────
        services.AddIdentity<AppUser, AppRole>(options =>
        {
            // Şifre politikası
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 1;

            // Lockout ayarları — 5 başarısız deneme → 15 dakika kilitle
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // Kullanıcı ayarları
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

            // E-posta doğrulama (şimdilik kapalı — öğrenme amaçlı)
            options.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // ── JWT Bearer Authentication ──────────────────────
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                // İmzalama anahtarını doğrula
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,

                // Issuer doğrulaması
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,

                // Audience doğrulaması
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,

                // Token süresi doğrulaması
                ValidateLifetime = true,

                // Saat farkı toleransı (varsayılan 5 dk → 0'a çek)
                ClockSkew = TimeSpan.Zero
            };

            // JWT token eventleri (debug/loglama amaçlı)
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    // Token süresi dolduysa header'a bilgi ekle
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers["Token-Expired"] = "true";
                    }
                    return Task.CompletedTask;
                }
            };
        });

        // ── Authorization Politikaları ──────────────────────
        services.AddAuthorization();

        // ── Servis Kayıtları ──────────────────────────────
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddHttpContextAccessor(); // IHttpContextAccessor DI kaydı

        return services;
    }
}
