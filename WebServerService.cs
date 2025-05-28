using System;
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
            WebRootPath = "wwwroot"
        });

        // Dodaj własnego loggera przekazującego logi do WPF
        builder.Logging.ClearProviders();
        builder.Logging.AddProvider(new TextBoxLoggerProvider(_logAction));

        builder.Services.AddRazorComponents().AddInteractiveServerComponents();

        var app = builder.Build();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAntiforgery();
        app.MapRazorComponents<eatery_manager_server.Components.App>().AddInteractiveServerRenderMode();

        _cts = new CancellationTokenSource();
        _host = app;

        Task.Run(() => app.RunAsync(_cts.Token));
    }

    public void Stop()
    {
        if (_cts != null)
        {
            _cts.Cancel();
            _cts = null;
            _host = null;
        }
    }
}

// Logger do przekazywania logów do WPF
public class TextBoxLogger : ILogger
{
    private readonly Action<string> _logAction; // Zmieniono nazwę pola, aby uniknąć niejednoznaczności

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
            _logAction?.Invoke($"[{logLevel}] {logMessage}"); // Zmieniono nazwę pola na _logAction
        }
    }
}

public class TextBoxLoggerProvider : ILoggerProvider
{
    private readonly Action<string> _logAction; // Renamed to avoid ambiguity  

    public TextBoxLoggerProvider(Action<string> logAction)
    {
        _logAction = logAction;
    }

    public ILogger CreateLogger(string categoryName) => new TextBoxLogger(_logAction);
    public void Dispose() { }
}
