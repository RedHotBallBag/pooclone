namespace LaunchBox.Core.Services;

/// <summary>
/// Abstraction for user interaction (dialogs, messages)
/// </summary>
public interface IUserInteractionService
{
    void ShowMessage(string title, string message);
    bool ShowConfirmation(string title, string message);
    string? ShowInputDialog(string title, string prompt, string? defaultValue = null);
}
