using LaunchBox.Core.Plugins;
using LaunchBox.Core.Repositories;
using LaunchBox.Core.Services;
using LaunchBox.Data;
using LaunchBox.Data.Repositories;
using LaunchBox.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
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

        // Initialize plugins
        await InitializePluginsAsync();

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

        // Logging
        services.AddLogging(builder =>
        {
            builder.AddSerilog(dispose: true);
        });

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

        // New services
        services.AddSingleton<IPluginManager, PluginManager>();
        services.AddSingleton<IEmulatorDetectionService, EmulatorDetectionService>();
        services.AddSingleton<IPlatformDetectionService, PlatformDetectionService>();

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

            // Delete and recreate database if corrupted (MVP approach)
            // For production, use proper migrations
            try
            {
                // Test if database is accessible
                await context.Database.CanConnectAsync();

                // Ensure all tables exist
                await context.Database.EnsureCreatedAsync();
            }
            catch
            {
                // If database is corrupted, delete and recreate
                Log.Warning("Database corrupted, recreating...");
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
            }

            // Initialize emulator presets
            var emulatorService = scope.ServiceProvider.GetRequiredService<IEmulatorService>();
            await emulatorService.InitializeEmulatorPresetsAsync();

            Log.Information("Database initialized successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to initialize database");
            MessageBox.Show($"Failed to initialize database: {ex.Message}\n\nPlease delete the database file at:\n{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LaunchBox", "launchbox.db")}",
                "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown();
        }
    }

    private async Task InitializePluginsAsync()
    {
        try
        {
            var pluginManager = _serviceProvider!.GetRequiredService<IPluginManager>();
            await pluginManager.LoadPluginsAsync();

            // Wire up plugin manager to launcher
            var gameLauncher = _serviceProvider.GetRequiredService<IGameLauncherService>();
            if (gameLauncher is GameLauncherService launcherService)
            {
                launcherService.SetPluginManager(pluginManager);
            }

            Log.Information("Plugins initialized successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to initialize plugins");
        }
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        try
        {
            var pluginManager = _serviceProvider?.GetService<IPluginManager>();
            if (pluginManager != null)
            {
                await pluginManager.UnloadPluginsAsync();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error unloading plugins");
        }

        _serviceProvider?.Dispose();
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}
