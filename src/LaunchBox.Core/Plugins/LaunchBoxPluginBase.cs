using LaunchBox.Core.Models;
using Microsoft.Extensions.Logging;

namespace LaunchBox.Core.Plugins;

/// <summary>
/// Base class for LaunchBox plugins with default implementations
/// </summary>
public abstract class LaunchBoxPluginBase : ILaunchBoxPlugin
{
    protected IPluginContext? Context { get; private set; }

    public abstract string Id { get; }
    public abstract string Name { get; }
    public abstract string Version { get; }
    public abstract string Author { get; }
    public abstract string Description { get; }

    public virtual Task OnLoadAsync(IPluginContext context)
    {
        Context = context;
        Context.Logger.LogInformation($"Plugin {Name} v{Version} loaded");
        return Task.CompletedTask;
    }

    public virtual Task OnUnloadAsync()
    {
        Context?.Logger.LogInformation($"Plugin {Name} unloaded");
        return Task.CompletedTask;
    }

    public virtual Task OnStartupAsync()
    {
        return Task.CompletedTask;
    }

    public virtual Task OnGameLaunchAsync(Game game, Emulator emulator)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnGameCloseAsync(Game game)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnShutdownAsync()
    {
        return Task.CompletedTask;
    }
}
