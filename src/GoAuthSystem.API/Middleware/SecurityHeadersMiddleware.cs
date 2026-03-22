namespace GoAuthSystem.API.Middleware;

/// <summary>
/// Güvenlik başlıkları middleware'i.
/// OWASP önerilerine uygun HTTP güvenlik header'larını ekler.
/// Her yanıta otomatik olarak eklenir.
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // ── Güvenlik Header'ları ──────────────────────────

        // MIME-type sniffing saldırılarını engelle
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";

        // Clickjacking saldırılarını engelle — sayfanın iframe içinde gösterilmesini engelle
        context.Response.Headers["X-Frame-Options"] = "DENY";

        // Modern tarayıcılarda XSS filtresi devre dışı bırak (eski filtre güvenlik açığı oluşturabilir)
        context.Response.Headers["X-XSS-Protection"] = "0";

        // Referrer bilgisini kısıtla — sadece aynı origin'e tam URL gönder
        context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        // İçerik güvenlik politikası — sadece kendi origin'inden kaynak yüklemeye izin ver
        context.Response.Headers["Content-Security-Policy"] = "default-src 'self'";

        // İzin yönetimi — hassas tarayıcı API'lerine erişimi kısıtla
        context.Response.Headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";

        await _next(context);
    }
}

/// <summary>
/// Middleware extension method.
/// </summary>
public static class SecurityHeadersMiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SecurityHeadersMiddleware>();
    }
}
