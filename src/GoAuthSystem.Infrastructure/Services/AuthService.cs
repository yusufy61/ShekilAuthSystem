using System.Security.Claims;
using GoAuthSystem.Domain.Entities;
using GoAuthSystem.Domain.Interfaces;
using GoAuthSystem.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GoAuthSystem.Infrastructure.Services;

/// <summary>
/// Kimlik doğrulama servisi implementasyonu.
/// Login, Register, Refresh Token Rotation, Revoke ve ChangePassword işlemleri.
/// Bu sınıf projenin en kritik bileşenidir.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly AppDbContext _context;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ITokenService tokenService,
        AppDbContext context,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Kullanıcı girişi.
    /// 1. E-posta ile kullanıcıyı bul
    /// 2. Hesap aktiflik kontrolü
    /// 3. Şifre doğrulama + lockout kontrolü
    /// 4. Başarılıysa access + refresh token üret
    /// </summary>
    public async Task<(string AccessToken, string RefreshToken, DateTime Expiration)> LoginAsync(
        string email, string password)
    {
        // 1. Kullanıcıyı bul — genel hata mesajı (e-posta varlığını sızdırma)
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning("Başarısız giriş denemesi: {Email} — kullanıcı bulunamadı", email);
            throw new UnauthorizedAccessException("E-posta veya şifre hatalı.");
        }

        // 2. Hesap aktiflik kontrolü
        if (!user.IsActive)
        {
            _logger.LogWarning("Pasif hesap giriş denemesi: {Email}", email);
            throw new UnauthorizedAccessException("Hesabınız pasif durumda. Lütfen yönetici ile iletişime geçin.");
        }

        // 3. Şifre doğrulama + lockout (5 başarısız deneme → 15 dk kilitle)
        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

        if (signInResult.IsLockedOut)
        {
            _logger.LogWarning("Hesap kilitlendi: {Email}", email);
            throw new UnauthorizedAccessException("Çok fazla başarısız giriş denemesi. Hesabınız geçici olarak kilitlendi.");
        }

        if (!signInResult.Succeeded)
        {
            _logger.LogWarning("Başarısız giriş denemesi: {Email} — hatalı şifre", email);
            throw new UnauthorizedAccessException("E-posta veya şifre hatalı.");
        }

        // 4. Token üret
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Refresh token'ı kullanıcıya bağla ve veritabanına kaydet
        refreshToken.AppUserId = user.Id;
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Başarılı giriş: {Email}", email);

        return (accessToken, refreshToken.Token, refreshToken.Expires);
    }

    /// <summary>
    /// Yeni kullanıcı kaydı.
    /// 1. E-posta benzersizlik kontrolü
    /// 2. Kullanıcı oluştur
    /// 3. Varsayılan "Student" rolü ata
    /// 4. Otomatik login yap ve token döndür
    /// </summary>
    public async Task<(string AccessToken, string RefreshToken, DateTime Expiration)> RegisterAsync(
        string firstName, string lastName, string email, string password, string role = "Student")
    {
        // 1. E-posta benzersizlik kontrolü — güvenli hata mesajı (e-posta varlığını sızdırma)
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            _logger.LogWarning("Kayıt denemesi: {Email} — e-posta zaten kayıtlı", email);
            throw new InvalidOperationException("Kayıt işlemi başarısız. Lütfen bilgilerinizi kontrol edin.");
        }

        // 2. Yeni kullanıcı oluştur
        var user = new AppUser
        {
            FirstName = firstName,
            LastName = lastName,
            UserName = email,       // UserName olarak e-posta kullanılır
            Email = email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Kullanıcı oluşturulamadı: {errors}");
        }

        // 3. Rol ata
        var roleResult = await _userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
        {
            // Kullanıcı oluşturuldu ama rol atanamadı — temizlik yap
            await _userManager.DeleteAsync(user);
            throw new InvalidOperationException("Rol ataması başarısız oldu.");
        }

        // 4. Token üret
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        refreshToken.AppUserId = user.Id;
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Yeni kullanıcı kaydı: {Email}, Rol: {Role}", email, role);

        return (accessToken, refreshToken.Token, refreshToken.Expires);
    }

    /// <summary>
    /// Refresh token rotation.
    /// 1. Süresi dolmuş access token'dan kullanıcıyı bul
    /// 2. Refresh token'ı veritabanında doğrula
    /// 3. REPLAY ATTACK KONTROLÜ
    /// 4. Eski token'ı revoke et, yeni token üret (rotation)
    /// </summary>
    public async Task<(string AccessToken, string RefreshToken, DateTime Expiration)> RefreshTokenAsync(
        string accessToken, string refreshToken)
    {
        // 1. Süresi dolmuş access token'dan ClaimsPrincipal çıkar
        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
        if (principal == null)
        {
            throw new UnauthorizedAccessException("Geçersiz access token.");
        }

        // Kullanıcı ID'sini claim'den al
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Token'dan kullanıcı bilgisi alınamadı.");
        }

        // 2. Kullanıcıyı ve refresh token'larını veritabanından getir
        var user = await _userManager.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Kullanıcı bulunamadı.");
        }

        // 3. Gönderilen refresh token'ı bul
        var existingToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
        if (existingToken == null)
        {
            throw new UnauthorizedAccessException("Geçersiz refresh token.");
        }

        // ── REPLAY ATTACK KONTROLÜ ──────────────────────
        // Eğer token zaten revoke edilmişse → replay attack!
        // Güvenlik önlemi: kullanıcının TÜM tokenlarını iptal et
        if (existingToken.IsRevoked)
        {
            _logger.LogWarning(
                "REPLAY ATTACK tespit edildi! Kullanıcı: {UserId}, Token: {Token}",
                userId, refreshToken);

            // Tüm aktif tokenları revoke et
            await RevokeAllTokensInternalAsync(user, "Replay attack tespit edildi — tüm tokenlar iptal edildi");
            await _context.SaveChangesAsync();

            throw new UnauthorizedAccessException("Güvenlik ihlali tespit edildi. Lütfen tekrar giriş yapın.");
        }

        // Token süresi dolmuş mu?
        if (existingToken.IsExpired)
        {
            throw new UnauthorizedAccessException("Refresh token süresi dolmuş. Lütfen tekrar giriş yapın.");
        }

        // 4. Rotation: eski token'ı revoke et, yeni token üret
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        newRefreshToken.AppUserId = user.Id;

        // Eski token'ı iptal et ve zincirle
        existingToken.Revoked = DateTime.UtcNow;
        existingToken.ReasonRevoked = "Yeni token ile değiştirildi (rotation)";
        existingToken.ReplacedByToken = newRefreshToken.Token;

        // Yeni token'ı veritabanına ekle
        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        // Yeni access token üret
        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = _tokenService.GenerateAccessToken(user, roles);

        _logger.LogInformation("Token rotation başarılı: Kullanıcı {UserId}", userId);

        return (newAccessToken, newRefreshToken.Token, newRefreshToken.Expires);
    }

    /// <summary>
    /// Belirli bir refresh token'ı iptal et (logout işlemi).
    /// </summary>
    public async Task RevokeTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (token == null)
        {
            throw new InvalidOperationException("Refresh token bulunamadı.");
        }

        if (token.IsRevoked)
        {
            throw new InvalidOperationException("Bu token zaten iptal edilmiş.");
        }

        token.Revoked = DateTime.UtcNow;
        token.ReasonRevoked = "Kullanıcı tarafından iptal edildi (logout)";
        await _context.SaveChangesAsync();

        _logger.LogInformation("Token iptal edildi: Kullanıcı {UserId}", token.AppUserId);
    }

    /// <summary>
    /// Kullanıcının TÜM aktif refresh token'larını iptal et.
    /// </summary>
    public async Task RevokeAllTokensAsync(int userId)
    {
        var user = await _userManager.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new InvalidOperationException("Kullanıcı bulunamadı.");
        }

        await RevokeAllTokensInternalAsync(user, "Tüm tokenlar kullanıcı/admin tarafından iptal edildi");
        await _context.SaveChangesAsync();

        _logger.LogInformation("Tüm tokenlar iptal edildi: Kullanıcı {UserId}", userId);
    }

    /// <summary>
    /// Şifre değiştirme.
    /// 1. Eski şifre doğrula
    /// 2. Yeni şifre güncelle
    /// 3. Tüm refresh token'ları revoke et (zorunlu yeniden giriş)
    /// 4. Security stamp güncelle
    /// </summary>
    public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new InvalidOperationException("Kullanıcı bulunamadı.");
        }

        // 1. Eski şifre doğrula + yeni şifre güncelle
        var changeResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!changeResult.Succeeded)
        {
            var errors = string.Join(", ", changeResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Şifre değiştirilemedi: {errors}");
        }

        // 2. Tüm refresh token'ları revoke et — zorunlu yeniden giriş
        await RevokeAllTokensInternalAsync(user, "Şifre değişikliği — tüm oturumlar sonlandırıldı");
        await _context.SaveChangesAsync();

        // 3. Security stamp güncelle — mevcut cookie/token'ları geçersiz kılar
        await _userManager.UpdateSecurityStampAsync(user);

        _logger.LogInformation("Şifre değiştirildi: Kullanıcı {UserId}", userId);
    }

    // ── Private Helper ──────────────────────────────

    /// <summary>
    /// Kullanıcının tüm aktif refresh token'larını revoke et (internal).
    /// SaveChanges çağırmaz — çağıran metot sorumludur.
    /// </summary>
    private Task RevokeAllTokensInternalAsync(AppUser user, string reason)
    {
        var activeTokens = user.RefreshTokens.Where(rt => rt.IsActive).ToList();

        foreach (var token in activeTokens)
        {
            token.Revoked = DateTime.UtcNow;
            token.ReasonRevoked = reason;
        }

        return Task.CompletedTask;
    }
}
