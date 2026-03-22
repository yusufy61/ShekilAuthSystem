namespace GoAuthSystem.Domain.Interfaces;

/// <summary>
/// Kimlik doğrulama (Authentication) servisi arayüzü.
/// Login, Register, Refresh Token, Revoke ve şifre işlemleri.
/// Somut implementasyon Infrastructure katmanında yapılacak.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Kullanıcı girişi — e-posta ve şifre ile doğrulama yapar,
    /// başarılıysa access token ve refresh token döner.
    /// </summary>
    /// <param name="email">Kullanıcı e-posta adresi</param>
    /// <param name="password">Kullanıcı şifresi</param>
    /// <returns>Access token, refresh token ve kullanıcı bilgilerini içeren tuple</returns>
    Task<(string AccessToken, string RefreshToken, DateTime Expiration)> LoginAsync(string email, string password);

    /// <summary>
    /// Yeni kullanıcı kaydı — kullanıcı oluşturur ve belirtilen role atar.
    /// </summary>
    /// <param name="firstName">Ad</param>
    /// <param name="lastName">Soyad</param>
    /// <param name="email">E-posta</param>
    /// <param name="password">Şifre</param>
    /// <param name="role">Atanacak rol (varsayılan: Student)</param>
    Task<(string AccessToken, string RefreshToken, DateTime Expiration)> RegisterAsync(
        string firstName, string lastName, string email, string password, string role = "Student");

    /// <summary>
    /// Refresh token rotation — süresi dolmuş access token ve geçerli refresh token ile
    /// yeni bir access token ve refresh token çifti üretir.
    /// Eski refresh token otomatik olarak iptal edilir (rotation).
    /// </summary>
    /// <param name="accessToken">Süresi dolmuş access token</param>
    /// <param name="refreshToken">Geçerli refresh token</param>
    Task<(string AccessToken, string RefreshToken, DateTime Expiration)> RefreshTokenAsync(
        string accessToken, string refreshToken);

    /// <summary>
    /// Refresh token iptal — belirtilen refresh token'ı geçersiz kılar.
    /// Logout işleminde veya güvenlik ihlallerinde kullanılır.
    /// </summary>
    /// <param name="refreshToken">İptal edilecek refresh token</param>
    Task RevokeTokenAsync(string refreshToken);

    /// <summary>
    /// Kullanıcının TÜM aktif refresh token'larını iptal et.
    /// Şifre değişikliği, hesap güvenliği ihlali durumlarında kullanılır.
    /// </summary>
    /// <param name="userId">Kullanıcı ID</param>
    Task RevokeAllTokensAsync(int userId);

    /// <summary>
    /// Şifre değiştirme — mevcut şifreyi doğrular ve yeni şifreyi ayarlar.
    /// </summary>
    /// <param name="userId">Kullanıcı ID</param>
    /// <param name="currentPassword">Mevcut şifre</param>
    /// <param name="newPassword">Yeni şifre</param>
    Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);
}
