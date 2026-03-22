using System.Security.Claims;
using GoAuthSystem.Domain.Entities;

namespace GoAuthSystem.Domain.Interfaces;

/// <summary>
/// JWT Token servisi arayüzü.
/// Access token ve refresh token üretme/doğrulama işlemleri.
/// Somut implementasyon Infrastructure katmanında yapılacak.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Kullanıcı ve rollere göre JWT access token üret.
    /// </summary>
    /// <param name="user">Kullanıcı entity'si</param>
    /// <param name="roles">Kullanıcının rolleri</param>
    /// <returns>JWT access token string</returns>
    string GenerateAccessToken(AppUser user, IList<string> roles);

    /// <summary>
    /// Rastgele refresh token üret (64 byte, base64 encoded).
    /// </summary>
    /// <returns>RefreshToken entity'si</returns>
    RefreshToken GenerateRefreshToken();

    /// <summary>
    /// Süresi dolmuş access token'dan ClaimsPrincipal çıkar.
    /// Refresh token rotation sırasında kullanıcıyı tanımlamak için kullanılır.
    /// </summary>
    /// <param name="token">Süresi dolmuş JWT token</param>
    /// <returns>ClaimsPrincipal veya null</returns>
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
