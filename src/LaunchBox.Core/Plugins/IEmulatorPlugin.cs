namespace LaunchBox.Core.Plugins;

/// <summary>
/// Plugin type categories
/// </summary>
public enum PluginType
{
    /// <summary>
    /// General purpose plugin
    /// </summary>
    General,

    /// <summary>
    /// Emulator extension plugin
    /// </summary>
    EmulatorExtension,

    /// <summary>
    /// Theme/UI plugin
    /// </summary>
    Theme,

    /// <summary>
    /// Media scraper plugin
    /// </summary>
    MediaScraper,

    /// <summary>
    /// Import/Export plugin
    /// </summary>
    ImportExport
}

/// <summary>
/// Marker interface for emulator extension plugins
/// </summary>
public interface IEmulatorPlugin : ILaunchBoxPlugin
{
    /// <summary>
    /// Plugin type
    /// </summary>
    PluginType PluginType => PluginType.EmulatorExtension;

    /// <summary>
    /// Emulator IDs this plugin supports (null = all emulators)
    /// </summary>
    IEnumerable<int>? SupportedEmulatorIds { get; }

    /// <summary>
    /// Check for emulator updates
    /// </summary>
    Task<bool> CheckForUpdatesAsync(int emulatorId);

    /// <summary>
    /// Update emulator
    /// </summary>
    Task<bool> UpdateEmulatorAsync(int emulatorId);

    /// <summary>
    /// Configure emulator settings
    /// </summary>
    Task ConfigureEmulatorAsync(int emulatorId);
}
