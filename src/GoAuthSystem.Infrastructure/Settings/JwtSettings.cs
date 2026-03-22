namespace GoAuthSystem.Infrastructure.Settings;

/// <summary>
/// JWT ayarları — Options pattern ile appsettings.json'dan okunur.
/// Program.cs'de: services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Token imzalama anahtarı — en az 64 karakter (HmacSha512 için)
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Token yayıncısı (issuer)
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Token hedef kitlesi (audience)
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Access token ömrü (dakika) — varsayılan 15 dakika
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; } = 15;

    /// <summary>
    /// Refresh token ömrü (gün) — varsayılan 7 gün
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; } = 7;
}
