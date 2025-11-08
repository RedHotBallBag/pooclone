namespace LaunchBox.Core.Plugins;

/// <summary>
/// Service for managing plugins
/// </summary>
public interface IPluginManager
{
    /// <summary>
    /// Load all plugins from plugin directories
    /// </summary>
    Task LoadPluginsAsync();

    /// <summary>
    /// Unload all plugins
    /// </summary>
    Task UnloadPluginsAsync();

    /// <summary>
    /// Get all loaded plugins
    /// </summary>
    IEnumerable<ILaunchBoxPlugin> GetLoadedPlugins();

    /// <summary>
    /// Get plugin by ID
    /// </summary>
    ILaunchBoxPlugin? GetPlugin(string pluginId);

    /// <summary>
    /// Get all emulator plugins
    /// </summary>
    IEnumerable<IEmulatorPlugin> GetEmulatorPlugins();

    /// <summary>
    /// Reload a specific plugin
    /// </summary>
    Task ReloadPluginAsync(string pluginId);

    /// <summary>
    /// Enable/disable a plugin
    /// </summary>
    Task SetPluginEnabledAsync(string pluginId, bool enabled);
}
