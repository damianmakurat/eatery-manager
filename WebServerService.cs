using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class WebServerService
{
    private IHost? _host;
    private CancellationTokenSource? _cts;
    private readonly Action<string> _logAction;

    // Konstruktor przyjmujący delegat do logowania
    public WebServerService(Action<string> logAction)
    {
        _logAction = logAction;
    }

    public void Start()
    {
        if (_host != null) return; // już działa

        string blazorProjectPath = @"C:\Users\Damian\GitHub\eatery-manager-server";

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ContentRootPath = blazorProjectPath,
            WebRootPath = Path.Combine(blazorProjectPath, "wwwroot"), // Pełna ścieżka do wwwroot
            EnvironmentName = "Development" // WAŻNE: Ustaw na Development
        });

        // Dodaj własnego loggera przekazującego logi do WPF
        builder.Logging.ClearProviders();
        builder.Logging.AddProvider(new TextBoxLoggerProvider(_logAction));
        builder.Logging.SetMinimumLevel(LogLevel.Debug); // Włącz szczegółowe logi

        builder.Services.AddRazorComponents().AddInteractiveServerComponents();

        var app = builder.Build();

        // WAŻNE: Dodaj obsługę błędów w Development
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        // Dodaj middleware do logowania wszystkich requestów
        app.Use(async (context, next) =>
        {
            _logAction?.Invoke($"[Debug] Request: {context.Request.Method} {context.Request.Path}");
            try
            {
                await next();
                _logAction?.Invoke($"[Debug] Response: {context.Response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logAction?.Invoke($"[Error] Unhandled exception in request {context.Request.Path}: {ex.Message}");
                _logAction?.Invoke($"[Error] Stack trace: {ex.StackTrace}");
                throw;
            }
        });

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAntiforgery();

        app.MapRazorComponents<eatery_manager_server.App>().AddInteractiveServerRenderMode();

        _cts = new CancellationTokenSource();
        _host = app;

        Task.Run(() => app.RunAsync(_cts.Token));
    }

    public async void Stop()
    {
        if (_cts != null)
        {
            _cts.Cancel();
        }

        if (_host != null)
        {
            try
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5)); // Daj 5 sekund na graceful shutdown
                _host.Dispose();
                _logAction?.Invoke("[Information] Server stopped successfully");
            }
            catch (Exception ex)
            {
                _logAction?.Invoke($"[Error] Error stopping server: {ex.Message}");
            }
            finally
            {
                _host = null;
                _cts = null;
            }
        }
    }
}

// Logger do przekazywania logów do WPF
public class TextBoxLogger : ILogger
{
    private readonly Action<string> _logAction;

    public TextBoxLogger(Action<string> logAction)
    {
        _logAction = logAction;
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (formatter != null)
        {
            var logMessage = formatter(state, exception);
            _logAction?.Invoke($"[{logLevel}] {logMessage}"); // POPRAWKA: usunięto * przed _logAction
        }
    }
}

public class TextBoxLoggerProvider : ILoggerProvider
{
    private readonly Action<string> _logAction;

    public TextBoxLoggerProvider(Action<string> logAction)
    {
        _logAction = logAction;
    }

    public ILogger CreateLogger(string categoryName) => new TextBoxLogger(_logAction);

    public void Dispose() { }
}