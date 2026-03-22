using GoAuthSystem.API.Data;
using GoAuthSystem.API.Extensions;
using GoAuthSystem.API.Middleware;
using GoAuthSystem.Application;
using GoAuthSystem.Infrastructure;
using GoAuthSystem.Persistence;

var builder = WebApplication.CreateBuilder(args);

// ══════════════════════════════════════════════════
// 1. SERVİS KAYITLARI (DI Container)
// ══════════════════════════════════════════════════

// Katman servis kayıtları — her katman kendi ServiceRegistration'ına sahip
builder.Services.AddApplicationServices();                                    // AutoMapper + FluentValidation
builder.Services.AddPersistenceServices(builder.Configuration);              // DbContext + Repository + UnitOfWork
builder.Services.AddInfrastructureServices(builder.Configuration);           // Identity + JWT + Auth/Token servisleri

// Controller'ları ekle
builder.Services.AddControllers();

// Swagger + JWT Bearer desteği
builder.Services.AddSwaggerWithJwt();

// CORS politikası (güvenli — belirli origin'ler)
builder.Services.AddCorsPolicy();

// Rate Limiting (IP bazlı — login:10/dk, register:5/dk, refresh:20/dk)
builder.Services.AddRateLimitingPolicies();

// API keşfi
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// ══════════════════════════════════════════════════
// 2. SEED DATA (Runtime — RoleManager/UserManager ile)
// ══════════════════════════════════════════════════

await DbInitializer.SeedAsync(app.Services);

// ══════════════════════════════════════════════════
// 3. MIDDLEWARE PIPELINE (Sıra önemli!)
// ══════════════════════════════════════════════════

// Global exception handling — TÜM hataları yakalar (ilk sırada olmalı)
app.UseExceptionHandling();

// Güvenlik header'ları — X-Content-Type-Options, X-Frame-Options, CSP vb.
app.UseSecurityHeaders();

// Request logging — her isteği loglar (method, path, status, duration)
app.UseRequestLogging();

// HSTS — production'da Strict-Transport-Security header'ı ekler
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// Swagger — sadece Development ortamında
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "GoAuthSystem API v1");
        options.RoutePrefix = string.Empty; // Swagger'ı root'ta aç (localhost:xxxx/)
    });
}

app.UseHttpsRedirection();

// CORS — güvenli politika (belirli origin'ler)
app.UseCors("SecurePolicy");

// Rate Limiting — IP bazlı istek sınırlandırması
app.UseRateLimiter();

// Authentication → Authorization (sıra önemli!)
app.UseAuthentication();
app.UseAuthorization();

// Controller routing
app.MapControllers();

app.Run();
