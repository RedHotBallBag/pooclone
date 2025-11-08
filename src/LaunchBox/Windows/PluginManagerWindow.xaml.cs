using LaunchBox.Core.Plugins;
using System.Diagnostics;
using System.Windows;

namespace LaunchBox.Windows;

public partial class PluginManagerWindow : Window
{
    private readonly IPluginManager _pluginManager;

    public PluginManagerWindow(IPluginManager pluginManager)
    {
        InitializeComponent();
        _pluginManager = pluginManager;
        Loaded += PluginManagerWindow_Loaded;
    }

    private void PluginManagerWindow_Loaded(object sender, RoutedEventArgs e)
    {
        LoadPlugins();
    }

    private void LoadPlugins()
    {
        var plugins = _pluginManager.GetLoadedPlugins().ToList();
        PluginsDataGrid.ItemsSource = plugins;
        PluginCountText.Text = $"{plugins.Count} plugin(s) loaded";
    }

    private void OpenPluginsFolder_Click(object sender, RoutedEventArgs e)
    {
        var pluginsPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
        System.IO.Directory.CreateDirectory(pluginsPath);
        Process.Start("explorer.exe", pluginsPath);
    }

    private async void ReloadPlugins_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await _pluginManager.UnloadPluginsAsync();
            await _pluginManager.LoadPluginsAsync();
            LoadPlugins();
            MessageBox.Show("Plugins reloaded successfully!", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to reload plugins: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
