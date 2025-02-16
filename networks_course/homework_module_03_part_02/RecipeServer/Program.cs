// Program.cs
using Microsoft.AspNetCore.RateLimiting;
using System.Collections.Concurrent;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddRateLimiter(options => {
    options.AddSlidingWindowLimiter("PerClientRateLimit", limiterOptions => {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromHours(1);
        limiterOptions.SegmentsPerWindow = 6;
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.AutoReplenishment = true;
        limiterOptions.QueueLimit = 0;
    });
});
// client tracking
builder.Services.AddSingleton<ClientTracker>();
builder.Services.AddHostedService<InactivityMonitor>();
// logging
builder.Logging.AddFile("Logs/server.log");
builder.Logging.AddConsole();

var app = builder.Build();

// mid
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseRateLimiter();
app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(name: "default",
                       pattern: "{controller=Home}/{action=Index}");

app.Run();

public class ClientTracker {
    private readonly ConcurrentDictionary<string, ClientInfo> _clients = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public bool TryAcquireSlot(string clientId) {
        if (_semaphore.Wait(0)) {
            _clients.TryAdd(clientId,
                            new ClientInfo { LastActivity = DateTime.UtcNow });
            return true;
        }
        return false;
    }

    public void ReleaseSlot(string clientId) {
        _clients.TryRemove(clientId, out _);
        _semaphore.Release();
    }

    public void UpdateActivity(string clientId) {
        if (_clients.TryGetValue(clientId, out var info)) {
            info.LastActivity = DateTime.UtcNow;
        }
    }

    public void CleanInactiveClients(TimeSpan timeout) {
        var cutoff = DateTime.UtcNow - timeout;
        foreach (var client in _clients) {
            if (client.Value.LastActivity < cutoff) {
                ReleaseSlot(client.Key);
            }
        }
    }
}

public class ClientInfo {
    public DateTime LastActivity { get; set; }
}

public class InactivityMonitor : BackgroundService {
    private readonly ClientTracker _tracker;
    private readonly ILogger<InactivityMonitor> _logger;

    public InactivityMonitor(ClientTracker tracker,
                             ILogger<InactivityMonitor> logger) {
        _tracker = tracker;
        _logger = logger;
    }

    protected override
        async Task ExecuteAsync(CancellationToken stoppingToken) {
        while (!stoppingToken.IsCancellationRequested) {
            _tracker.CleanInactiveClients(TimeSpan.FromMinutes(1));
            _logger.LogInformation("Cleaned inactive clients");
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}

public class RequestLoggingMiddleware {
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly ClientTracker _tracker;

    public RequestLoggingMiddleware(RequestDelegate next,
                                    ILogger<RequestLoggingMiddleware> logger,
                                    ClientTracker tracker) {
        _next = next;
        _logger = logger;
        _tracker = tracker;
    }

    public async Task Invoke(HttpContext context) {
        var clientId =
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        if (!_tracker.TryAcquireSlot(clientId)) {
            context.Response.StatusCode =
                StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsync(
                "Too many concurrent connections");
            _logger.LogWarning("Connection limit exceeded for {ClientId}",
                               clientId);
            return;
        }

        try {
            _tracker.UpdateActivity(clientId);

            var logMessage =
                $"[{DateTime.UtcNow:u}] {clientId} {context.Request.Method} {context.Request.Path}";
            _logger.LogInformation(logMessage);

            await _next(context);
        } finally {
            _tracker.ReleaseSlot(clientId);
        }
    }
}

public static class FileLoggerExtensions {
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder,
                                          string filePath) {
        builder.AddProvider(new FileLoggerProvider(filePath));
        return builder;
    }
}

public class FileLoggerProvider : ILoggerProvider {
    private readonly string _filePath;

    public FileLoggerProvider(string filePath) {
        _filePath = filePath;
    }

    public ILogger
    CreateLogger(string categoryName) => new FileLogger(_filePath);

    public void Dispose() {
    }
}

public class FileLogger : ILogger {
    private readonly string _filePath;

    public FileLogger(string filePath) {
        _filePath = filePath;
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
                            Exception exception,
                            Func<TState, Exception, string> formatter) {
        File.AppendAllText(
            _filePath,
            $"{DateTime.UtcNow:u} [{logLevel}] {formatter(state, exception)}\n");
    }
}
