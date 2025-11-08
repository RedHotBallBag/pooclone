using LaunchBox.Core.Models;
using LaunchBox.Core.Services;
using Microsoft.Extensions.Logging;

namespace LaunchBox.Core.Plugins;

/// <summary>
/// Plugin context providing access to LaunchBox APIs
/// </summary>
public interface IPluginContext
{
    /// <summary>
    /// Plugin-specific logger
    /// </summary>
    ILogger Logger { get; }

    /// <summary>
    /// Plugin data directory
    /// </summary>
    string DataDirectory { get; }

    /// <summary>
    /// Game library service
    /// </summary>
    IGameLibraryService GameLibrary { get; }

    /// <summary>
    /// Emulator service
    /// </summary>
    IEmulatorService EmulatorService { get; }

    /// <summary>
    /// Game launcher service
    /// </summary>
    IGameLauncherService GameLauncher { get; }

    /// <summary>
    /// Media scraper service
    /// </summary>
    IMediaScraperService MediaScraper { get; }

    /// <summary>
    /// Get plugin setting
    /// </summary>
    T? GetSetting<T>(string key, T? defaultValue = default);

    /// <summary>
    /// Set plugin setting
    /// </summary>
    void SetSetting<T>(string key, T value);

    /// <summary>
    /// Show message to user
    /// </summary>
    void ShowMessage(string title, string message);

    /// <summary>
    /// Show confirmation dialog
    /// </summary>
    bool ShowConfirmation(string title, string message);
}
