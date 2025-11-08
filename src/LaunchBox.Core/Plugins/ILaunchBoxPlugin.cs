using LaunchBox.Core.Models;

namespace LaunchBox.Core.Plugins;

/// <summary>
/// Base interface for all LaunchBox plugins
/// </summary>
public interface ILaunchBoxPlugin
{
    /// <summary>
    /// Plugin unique identifier
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Plugin display name
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Plugin version
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Plugin author
    /// </summary>
    string Author { get; }

    /// <summary>
    /// Plugin description
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Called when the plugin is loaded
    /// </summary>
    Task OnLoadAsync(IPluginContext context);

    /// <summary>
    /// Called when the plugin is unloaded
    /// </summary>
    Task OnUnloadAsync();

    /// <summary>
    /// Called when LaunchBox starts
    /// </summary>
    Task OnStartupAsync();

    /// <summary>
    /// Called when a game is about to launch
    /// </summary>
    Task OnGameLaunchAsync(Game game, Emulator emulator);

    /// <summary>
    /// Called when a game is closed
    /// </summary>
    Task OnGameCloseAsync(Game game);

    /// <summary>
    /// Called when LaunchBox is shutting down
    /// </summary>
    Task OnShutdownAsync();
}
