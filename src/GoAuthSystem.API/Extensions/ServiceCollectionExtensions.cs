using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;

namespace GoAuthSystem.API.Extensions;

/// <summary>
/// DI container extension metotları.
/// Swagger, CORS ve Rate Limiting konfigürasyonlarını düzenli tutar.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Swagger + JWT Bearer desteği.
    /// Swagger UI'da "Authorize" butonu ile JWT token girilebilir.
    /// </summary>
    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "GoAuthSystem API",
                Version = "v1",
                Description = "ASP.NET Core 8 — JWT Authentication & Authorization Sistemi",
                Contact = new OpenApiContact
                {
                    Name = "GoAthletic",
                    Email = "admin@goathletic.com"
                }
            });

            // JWT Bearer token girişi için "Authorize" butonu
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT token'ınızı girin.\n\nÖrnek: eyJhbGciOiJIUz..."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    /// <summary>
    /// CORS politikası — belirli origin'lere izin ver (wildcard * KULLANMA).
    /// AllowCredentials ile cookie/header güvenliği sağlanır.
    /// </summary>
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("SecurePolicy", policy =>
            {
                policy
                    .WithOrigins(
                        "https://localhost:3000",    // Frontend dev
                        "https://localhost:5173",    // Vite dev
                        "https://goathletic.com",    // Production
                        "https://www.goathletic.com" // Production www
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()                    // Cookie/Header güvenliği
                    .WithExposedHeaders("Token-Expired");  // JWT expired header'ını dışa aç
            });
        });

        return services;
    }

    /// <summary>
    /// Rate Limiting — ASP.NET Core 8 built-in.
    /// IP bazlı istek sınırlandırması:
    ///   - login: 10 istek/dakika
    ///   - register: 5 istek/dakika
    ///   - refresh: 20 istek/dakika
    ///   - global: 100 istek/dakika (genel limit)
    /// </summary>
    public static IServiceCollection AddRateLimitingPolicies(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // 429 Too Many Requests yanıtı
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Login endpoint — dakikada max 10 istek (IP bazlı)
            options.AddFixedWindowLimiter("login", limiter =>
            {
                limiter.PermitLimit = 10;
                limiter.Window = TimeSpan.FromMinutes(1);
                limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiter.QueueLimit = 0; // Kuyruk yok, direkt reddet
            });

            // Register endpoint — dakikada max 5 istek
            options.AddFixedWindowLimiter("register", limiter =>
            {
                limiter.PermitLimit = 5;
                limiter.Window = TimeSpan.FromMinutes(1);
                limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiter.QueueLimit = 0;
            });

            // Refresh token endpoint — dakikada max 20 istek
            options.AddFixedWindowLimiter("refresh", limiter =>
            {
                limiter.PermitLimit = 20;
                limiter.Window = TimeSpan.FromMinutes(1);
                limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiter.QueueLimit = 0;
            });

            // Global rate limit — dakikada max 100 istek (tüm endpointler)
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1)
                    });
            });
        });

        return services;
    }
}
