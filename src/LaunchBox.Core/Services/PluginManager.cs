using LaunchBox.Core.Plugins;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Runtime.Loader;

namespace LaunchBox.Core.Services;

public class PluginManager : IPluginManager
{
    private readonly ILogger<PluginManager> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _pluginsDirectory;
    private readonly Dictionary<string, PluginInfo> _loadedPlugins = new();
    private readonly Dictionary<string, AssemblyLoadContext> _pluginContexts = new();

    public PluginManager(ILogger<PluginManager> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _pluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
        Directory.CreateDirectory(_pluginsDirectory);
    }

    public async Task LoadPluginsAsync()
    {
        _logger.LogInformation("Loading plugins from {PluginsDirectory}", _pluginsDirectory);

        var pluginDlls = Directory.GetFiles(_pluginsDirectory, "*.dll", SearchOption.AllDirectories);

        foreach (var dllPath in pluginDlls)
        {
            try
            {
                await LoadPluginFromAssemblyAsync(dllPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load plugin from {DllPath}", dllPath);
            }
        }

        _logger.LogInformation("Loaded {Count} plugins", _loadedPlugins.Count);

        // Call OnStartupAsync for all plugins
        foreach (var pluginInfo in _loadedPlugins.Values.Where(p => p.IsEnabled))
        {
            try
            {
                await pluginInfo.Plugin.OnStartupAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Plugin {PluginName} OnStartupAsync failed", pluginInfo.Plugin.Name);
            }
        }
    }

    private async Task LoadPluginFromAssemblyAsync(string assemblyPath)
    {
        var loadContext = new AssemblyLoadContext(Path.GetFileName(assemblyPath), isCollectible: true);

        try
        {
            var assembly = loadContext.LoadFromAssemblyPath(assemblyPath);
            var pluginTypes = assembly.GetTypes()
                .Where(t => typeof(ILaunchBoxPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var pluginType in pluginTypes)
            {
                var plugin = (ILaunchBoxPlugin?)Activator.CreateInstance(pluginType);
                if (plugin == null) continue;

                // Create plugin context
                var context = new PluginContext(plugin.Id, _serviceProvider, _logger);

                // Initialize plugin
                await plugin.OnLoadAsync(context);

                var pluginInfo = new PluginInfo
                {
                    Plugin = plugin,
                    AssemblyPath = assemblyPath,
                    LoadContext = loadContext,
                    IsEnabled = true
                };

                _loadedPlugins[plugin.Id] = pluginInfo;
                _pluginContexts[plugin.Id] = loadContext;

                _logger.LogInformation("Loaded plugin: {PluginName} v{Version} by {Author}",
                    plugin.Name, plugin.Version, plugin.Author);
            }
        }
        catch (Exception ex)
        {
            loadContext.Unload();
            throw new InvalidOperationException($"Failed to load plugin assembly: {assemblyPath}", ex);
        }
    }

    public async Task UnloadPluginsAsync()
    {
        foreach (var pluginInfo in _loadedPlugins.Values)
        {
            try
            {
                await pluginInfo.Plugin.OnShutdownAsync();
                await pluginInfo.Plugin.OnUnloadAsync();
                pluginInfo.LoadContext?.Unload();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unload plugin {PluginId}", pluginInfo.Plugin.Id);
            }
        }

        _loadedPlugins.Clear();
        _pluginContexts.Clear();
    }

    public IEnumerable<ILaunchBoxPlugin> GetLoadedPlugins()
    {
        return _loadedPlugins.Values
            .Where(p => p.IsEnabled)
            .Select(p => p.Plugin);
    }

    public ILaunchBoxPlugin? GetPlugin(string pluginId)
    {
        return _loadedPlugins.TryGetValue(pluginId, out var info) && info.IsEnabled
            ? info.Plugin
            : null;
    }

    public IEnumerable<IEmulatorPlugin> GetEmulatorPlugins()
    {
        return _loadedPlugins.Values
            .Where(p => p.IsEnabled && p.Plugin is IEmulatorPlugin)
            .Select(p => (IEmulatorPlugin)p.Plugin);
    }

    public async Task ReloadPluginAsync(string pluginId)
    {
        if (!_loadedPlugins.TryGetValue(pluginId, out var pluginInfo))
        {
            throw new InvalidOperationException($"Plugin {pluginId} not found");
        }

        // Unload
        await pluginInfo.Plugin.OnUnloadAsync();
        pluginInfo.LoadContext?.Unload();

        _loadedPlugins.Remove(pluginId);
        _pluginContexts.Remove(pluginId);

        // Reload
        await LoadPluginFromAssemblyAsync(pluginInfo.AssemblyPath);
    }

    public Task SetPluginEnabledAsync(string pluginId, bool enabled)
    {
        if (_loadedPlugins.TryGetValue(pluginId, out var pluginInfo))
        {
            pluginInfo.IsEnabled = enabled;
            _logger.LogInformation("Plugin {PluginId} {Status}", pluginId, enabled ? "enabled" : "disabled");
        }

        return Task.CompletedTask;
    }

    private class PluginInfo
    {
        public required ILaunchBoxPlugin Plugin { get; init; }
        public required string AssemblyPath { get; init; }
        public AssemblyLoadContext? LoadContext { get; init; }
        public bool IsEnabled { get; set; }
    }
}
