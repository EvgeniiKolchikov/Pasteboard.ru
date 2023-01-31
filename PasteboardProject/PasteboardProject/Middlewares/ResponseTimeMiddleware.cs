using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using NLog.Fluent;

namespace PasteboardProject.Middlewares;

public class ResponseTimeMiddleware
{
    private readonly RequestDelegate _next;
    private Stopwatch _stopwatch;
    public ResponseTimeMiddleware(RequestDelegate next)
    {
        _next = next;
        _stopwatch = new Stopwatch();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _stopwatch.Start();
        await _next.Invoke(context);
        _stopwatch.Stop();
        Console.WriteLine($"Время запроса: {_stopwatch.ElapsedMilliseconds}");
        _stopwatch.Reset();
    }
}