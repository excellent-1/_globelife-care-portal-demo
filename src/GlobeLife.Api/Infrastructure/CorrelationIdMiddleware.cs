using System.Diagnostics;

namespace GlobeLife.Api.Infrastructure;

public sealed class CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue("X-Correlation-ID", out var supplied) && !string.IsNullOrWhiteSpace(supplied)
            ? supplied.ToString()
            : Guid.NewGuid().ToString("N");
        context.TraceIdentifier = correlationId;
        context.Response.Headers["X-Correlation-ID"] = correlationId;
        using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId, ["RequestPath"] = context.Request.Path.ToString() }))
        {
            var stopwatch = Stopwatch.StartNew();
            try { await next(context); }
            finally { stopwatch.Stop(); logger.LogInformation("HTTP {Method} {Path} completed with {StatusCode} in {ElapsedMilliseconds}ms", context.Request.Method, context.Request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds); }
        }
    }
}
