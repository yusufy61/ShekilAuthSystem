using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GoAuthSystem.Domain.Entities;
using GoAuthSystem.Domain.Interfaces;
using GoAuthSystem.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GoAuthSystem.Infrastructure.Services;

/// <summary>
/// JWT Token servisi implementasyonu.
/// Access token (HmacSha512) ve refresh token (RandomNumberGenerator) üretir.
/// </summary>
public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;

    public TokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    /// <summary>
    /// JWT access token üret — HmacSha512 imzalama, 15 dakika ömür.
    /// Claims: NameIdentifier, Email, Name, Role, TrainerId, TraineeId, Jti
    /// </summary>
    public string GenerateAccessToken(AppUser user, IList<string> roles)
    {
        // İmzalama anahtarı
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        // Claim listesi
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.FullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        // Rol claim'leri ekle
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Trainer/Trainee ID claim'leri (varsa)
        if (user.TrainerId.HasValue)
            claims.Add(new Claim("TrainerId", user.TrainerId.Value.ToString()));

        if (user.TraineeId.HasValue)
            claims.Add(new Claim("TraineeId", user.TraineeId.Value.ToString()));

        // Token oluştur
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Rastgele refresh token üret — 64 byte, base64 encoded, 7 gün ömür.
    /// Veritabanında RefreshToken tablosunda saklanır.
    /// </summary>
    public RefreshToken GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomBytes),
            Created = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        };
    }

    /// <summary>
    /// Süresi dolmuş access token'dan ClaimsPrincipal çıkar.
    /// ValidateLifetime = false → süresi dolmuş tokenlar da kabul edilir.
    /// Refresh token rotation sırasında kullanıcıyı tanımlamak için kullanılır.
    /// </summary>
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false, // Süresi dolmuş tokenları kabul et
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = key
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            // Token'ın gerçekten JWT ve doğru algoritma ile imzalanmış olduğunu kontrol et
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            // Token doğrulaması başarısız olursa null döndür
            return null;
        }
    }
}
