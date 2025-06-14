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
using eatery_manager;
using Microsoft.AspNetCore.Components.Authorization;

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
        if (_host != null) return;

        var settings = AppSettings.Load();
        if (settings == null)
        {
            _logAction?.Invoke("[Error] Nie udało się załadować ustawień aplikacji.");
            return;
        }

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ContentRootPath = settings.mainFilesPath,
            WebRootPath = Path.Combine(settings.mainFilesPath, "wwwroot"),
            EnvironmentName = "Development"
        });

        builder.Logging.ClearProviders();
        builder.Logging.AddProvider(new TextBoxLoggerProvider(_logAction));
        builder.Logging.SetMinimumLevel(LogLevel.Debug);

        string dataFolder = Path.Combine(settings.mainFilesPath, "Data");
        string dbPath = Path.Combine(dataFolder, "database.db");

        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
            _logAction?.Invoke($"[Information] Created Data directory: {dataFolder}");
        }

        string connectionString = $"Data Source={dbPath};Mode=ReadWriteCreate;Cache=Shared;Foreign Keys=True";
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        builder.Services.AddRazorComponents().AddInteractiveServerComponents();

        builder.Services.AddScoped<LoginService>();
        builder.Services.AddScoped<MenuService>();
        builder.Services.AddScoped<ReservationsService>();
        builder.Services.AddScoped<TablesService>();

        builder.Services.AddScoped<CustomAuthenticationStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
            sp.GetRequiredService<CustomAuthenticationStateProvider>());
        builder.Services.AddAuthorizationCore();

        builder.Services.AddServerSideBlazor(options =>
        {
            options.DetailedErrors = true;
        });

        var app = builder.Build();

        try
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.EnsureCreated();
            DatabaseInitializer.SeedAsync(dbContext).Wait();
            _logAction?.Invoke("[Information] Database initialized successfully");
        }
        catch (Exception ex)
        {
            _logAction?.Invoke($"[Error] Database init failed: {ex.Message}");
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAntiforgery();

        app.MapRazorComponents<eatery_manager_server.App>()
            .AddInteractiveServerRenderMode();

        // Dynamika portów z konfiguracji
        if (settings.httpState)
            app.Urls.Add($"http://localhost:{settings.httpPortListen}");
        if (settings.httpsState)
            app.Urls.Add($"https://localhost:{settings.httpsPortListen}");

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
public class TextBoxLogger : ILogger, IDisposable
{
    private readonly Action<string> _logAction;
    private readonly StreamWriter _fileWriter;

    public TextBoxLogger(Action<string> logAction, StreamWriter fileWriter)
    {
        _logAction = logAction;
        _fileWriter = fileWriter;
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (formatter != null)
        {
            var logMessage = formatter(state, exception);
            var finalMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{logLevel}] {logMessage}";

            // Log do WPF TextBox
            _logAction?.Invoke(finalMessage);

            // Log do pliku
            try
            {
                _fileWriter.WriteLine(finalMessage);
                _fileWriter.Flush();
            }
            catch
            {
                // Nie rzucamy dalej, żeby logowanie nie przerywało działania
            }
        }
    }

    public void Dispose()
    {
        _fileWriter?.Dispose();
    }
}

public class TextBoxLoggerProvider : ILoggerProvider
{
    private readonly Action<string> _logAction;
    private readonly StreamWriter _fileWriter;

    public TextBoxLoggerProvider(Action<string> logAction)
    {
        _logAction = logAction;

        // Nazwa pliku logu z datą i godziną startu
        string logsDir = Path.Combine(AppContext.BaseDirectory, "Logs");
        if (!Directory.Exists(logsDir))
            Directory.CreateDirectory(logsDir);

        string logFileName = $"logs_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";
        string logFilePath = Path.Combine(logsDir, logFileName);

        _fileWriter = new StreamWriter(new FileStream(logFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
        {
            AutoFlush = true
        };
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new TextBoxLogger(_logAction, _fileWriter);
    }

    public void Dispose()
    {
        _fileWriter?.Dispose();
    }
}