using LaunchBox.Core.Repositories;
using LaunchBox.Core.Services;
using LaunchBox.Data;
using LaunchBox.Data.Repositories;
using LaunchBox.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Windows;

namespace LaunchBox;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Configure logging
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("logs/launchbox.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // Configure services
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        // Initialize database
        await InitializeDatabaseAsync();

        // Show main window
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(ServiceCollection services)
    {
        // Database
        services.AddDbContext<LaunchBoxDbContext>(options =>
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "LaunchBox",
                "launchbox.db"
            );
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
            options.UseSqlite($"Data Source={dbPath}");
        });

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IGameRepository, GameRepository>();

        // Services
        services.AddScoped<IGameLibraryService, GameLibraryService>();
        services.AddScoped<IRomScannerService, RomScannerService>();
        services.AddScoped<IEmulatorService, EmulatorService>();
        services.AddScoped<IGameLauncherService, GameLauncherService>();
        services.AddScoped<IMediaScraperService>(sp => new MediaScraperService(
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "LaunchBox",
                "Media"
            )
        ));

        // ViewModels
        services.AddTransient<MainWindowViewModel>();

        // Views
        services.AddTransient<MainWindow>();
    }

    private async Task InitializeDatabaseAsync()
    {
        try
        {
            using var scope = _serviceProvider!.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<LaunchBoxDbContext>();

            // Apply migrations
            await context.Database.MigrateAsync();

            // Initialize emulator presets
            var emulatorService = scope.ServiceProvider.GetRequiredService<IEmulatorService>();
            await emulatorService.InitializeEmulatorPresetsAsync();

            Log.Information("Database initialized successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to initialize database");
            MessageBox.Show($"Failed to initialize database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}
