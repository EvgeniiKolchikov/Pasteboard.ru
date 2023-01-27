using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using NLog.Fluent;

namespace PasteboardProject.Middlewares;

public class ResponseTimeMiddleware
{
    private readonly RequestDelegate _next;

    public ResponseTimeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        await _next.Invoke(context);
        stopwatch.Stop();
        Console.WriteLine($"Время запроса: {stopwatch.ElapsedMilliseconds}");
    }
}