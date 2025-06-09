using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using eatery_manager_server.Data.Db;
using eatery_manager_server.Data.Services;

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
            WebRootPath = Path.Combine(blazorProjectPath, "wwwroot"),
            EnvironmentName = "Development"
        });

        // Dodaj własnego loggera przekazującego logi do WPF
        builder.Logging.ClearProviders();
        builder.Logging.AddProvider(new TextBoxLoggerProvider(_logAction));
        builder.Logging.SetMinimumLevel(LogLevel.Debug);

        // Konfiguracja bazy danych - WAŻNE: musi być przed innymi serwisami
        string dataFolder = Path.Combine(blazorProjectPath, "Data");
        string dbPath = Path.Combine(dataFolder, "database.db");

        // Upewnij się, że folder Data istnieje
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
            _logAction?.Invoke($"[Information] Created Data directory: {dataFolder}");
        }

        _logAction?.Invoke($"[Information] Database path: {dbPath}");
        _logAction?.Invoke($"[Information] Database exists: {File.Exists(dbPath)}");

        // Sprawdź czy plik nie jest zablokowany
        try
        {
            using (var fileStream = File.Open(dbPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                _logAction?.Invoke("[Information] Database file is accessible");
            }
        }
        catch (Exception ex)
        {
            _logAction?.Invoke($"[Warning] Database file access test failed: {ex.Message}");
        }

        // Użyj pełnej ścieżki w connection string z dodatkowymi opcjami
        string connectionString = $"Data Source={dbPath};Mode=ReadWriteCreate;Cache=Shared;Foreign Keys=True";
        _logAction?.Invoke($"[Information] Connection string: {connectionString}");

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        // Dodaj Razor Components
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();

        // Dodaj wszystkie serwisy jak w oryginalnym Program.cs
        builder.Services.AddScoped<LoginService>();
        builder.Services.AddScoped<MenuService>();

        // Dodaj Server-Side Blazor z szczegółowymi błędami
        builder.Services.AddServerSideBlazor(options =>
        {
            options.DetailedErrors = true;
        });

        var app = builder.Build();

        // Upewnij się, że baza danych jest utworzona
        try
        {
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Database.EnsureCreated(); // Tworzy bazę danych jeśli nie istnieje
                _logAction?.Invoke("[Information] Database initialized successfully");
            }
        }
        catch (Exception ex)
        {
            _logAction?.Invoke($"[Error] Database initialization failed: {ex.Message}");
        }

        // Obsługa błędów
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        // Middleware do logowania requestów z lepszą obsługą błędów
        app.Use(async (context, next) =>
        {
            _logAction?.Invoke($"[Debug] Request: {context.Request.Method} {context.Request.Path}");
            try
            {
                await next();
                _logAction?.Invoke($"[Debug] Response: {context.Response.StatusCode}");
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("There is no registered service"))
            {
                _logAction?.Invoke($"[Error] DI Registration Error: {ex.Message}");
                _logAction?.Invoke($"[Error] Sprawdź czy wszystkie serwisy są zarejestrowane w Program.cs lub WebServerService");
                throw;
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
                await _host.StopAsync(TimeSpan.FromSeconds(5));
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
            _logAction?.Invoke($"[{logLevel}] {logMessage}");
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