using LaunchBox.Core.Services;
using System.Windows;

namespace LaunchBox.Services;

public class WpfUserInteractionService : IUserInteractionService
{
    public void ShowMessage(string title, string message)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public bool ShowConfirmation(string title, string message)
    {
        var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        return result == MessageBoxResult.Yes;
    }

    public string? ShowInputDialog(string title, string prompt, string? defaultValue = null)
    {
        // TODO: Implement custom WPF input dialog
        // For now, just show a message and return default value
        MessageBox.Show($"{prompt}\n\n(Input dialog not yet implemented - would return: {defaultValue})",
            title, MessageBoxButton.OK, MessageBoxImage.Information);
        return defaultValue;
    }
}
