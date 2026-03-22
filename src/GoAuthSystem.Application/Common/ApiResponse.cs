namespace GoAuthSystem.Application.Common;

/// <summary>
/// Standart API yanıt wrapper'ı.
/// Tüm controller'lar bu formatta yanıt döner:
/// { success: true/false, message: "...", data: {...}, errors: [...] }
/// </summary>
/// <typeparam name="T">Yanıt veri tipi</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// İşlem başarılı mı?
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Kullanıcıya gösterilecek mesaj
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Yanıt verisi (başarılı işlemlerde dolu)
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Hata detayları (başarısız işlemlerde dolu)
    /// </summary>
    public List<string>? Errors { get; set; }

    // ── Factory Method'lar ──────────────────────────

    /// <summary>
    /// Başarılı yanıt oluştur (veri ile)
    /// </summary>
    public static ApiResponse<T> Ok(T data, string message = "İşlem başarılı.")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    /// <summary>
    /// Başarılı yanıt oluştur (veri olmadan)
    /// </summary>
    public static ApiResponse<T> Ok(string message = "İşlem başarılı.")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message
        };
    }

    /// <summary>
    /// Başarısız yanıt oluştur
    /// </summary>
    public static ApiResponse<T> Fail(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}

/// <summary>
/// Generic olmayan ApiResponse — veri döndürmeyen endpointler için.
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    /// <summary>
    /// Başarılı yanıt (veri olmadan)
    /// </summary>
    public new static ApiResponse Ok(string message = "İşlem başarılı.")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message
        };
    }

    /// <summary>
    /// Başarısız yanıt
    /// </summary>
    public new static ApiResponse Fail(string message, List<string>? errors = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}
