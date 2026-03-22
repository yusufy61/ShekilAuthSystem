using System.Diagnostics;

namespace GoAuthSystem.API.Middleware;

/// <summary>
/// Request logging middleware (opsiyonel).
/// Her isteği loglar: Method, Path, StatusCode, Duration.
/// Audit trail ve performans izleme için kullanılır.
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        // İsteği logla
        _logger.LogInformation(
            "HTTP {Method} {Path} başladı",
            context.Request.Method,
            context.Request.Path);

        await _next(context);

        stopwatch.Stop();

        // Yanıtı logla
        _logger.LogInformation(
            "HTTP {Method} {Path} → {StatusCode} ({Duration}ms)",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds);
    }
}

/// <summary>
/// Middleware extension method.
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }
}
