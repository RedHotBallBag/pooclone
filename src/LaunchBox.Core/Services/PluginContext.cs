using LaunchBox.Core.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LaunchBox.Core.Services;

internal class PluginContext : IPluginContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, object> _settings = new();
    private readonly string _settingsFilePath;

    public ILogger Logger { get; }
    public string DataDirectory { get; }

    public IGameLibraryService GameLibrary => _serviceProvider.GetRequiredService<IGameLibraryService>();
    public IEmulatorService EmulatorService => _serviceProvider.GetRequiredService<IEmulatorService>();
    public IGameLauncherService GameLauncher => _serviceProvider.GetRequiredService<IGameLauncherService>();
    public IMediaScraperService MediaScraper => _serviceProvider.GetRequiredService<IMediaScraperService>();

    public PluginContext(string pluginId, IServiceProvider serviceProvider, ILogger parentLogger)
    {
        _serviceProvider = serviceProvider;

        // Create plugin-specific logger
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        Logger = loggerFactory.CreateLogger($"Plugin.{pluginId}");

        // Set up plugin data directory
        DataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "LaunchBox",
            "Plugins",
            pluginId
        );
        Directory.CreateDirectory(DataDirectory);

        // Load settings
        _settingsFilePath = Path.Combine(DataDirectory, "settings.json");
        LoadSettings();
    }

    public T? GetSetting<T>(string key, T? defaultValue = default)
    {
        if (_settings.TryGetValue(key, out var value))
        {
            if (value is JsonElement jsonElement)
            {
                return jsonElement.Deserialize<T>();
            }
            return (T?)value;
        }

        return defaultValue;
    }

    public void SetSetting<T>(string key, T value)
    {
        if (value != null)
        {
            _settings[key] = value;
            SaveSettings();
        }
    }

    public void ShowMessage(string title, string message)
    {
        System.Windows.MessageBox.Show(message, title, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
    }

    public bool ShowConfirmation(string title, string message)
    {
        var result = System.Windows.MessageBox.Show(message, title, System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
        return result == System.Windows.MessageBoxResult.Yes;
    }

    private void LoadSettings()
    {
        try
        {
            if (File.Exists(_settingsFilePath))
            {
                var json = File.ReadAllText(_settingsFilePath);
                var loaded = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                if (loaded != null)
                {
                    foreach (var kvp in loaded)
                    {
                        _settings[kvp.Key] = kvp.Value;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load plugin settings");
        }
    }

    private void SaveSettings()
    {
        try
        {
            var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_settingsFilePath, json);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to save plugin settings");
        }
    }
}
