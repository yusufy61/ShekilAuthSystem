namespace GoAuthSystem.Application.DTOs.Auth;

/// <summary>
/// Refresh token iptal isteği DTO'su.
/// Logout sırasında veya güvenlik ihlalinde refresh token iptal edilir.
/// </summary>
public class RevokeTokenRequestDTO
{
    /// <summary>
    /// İptal edilecek refresh token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}
