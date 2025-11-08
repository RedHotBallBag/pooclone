using LaunchBox.Core.Models;
using LaunchBox.Core.Repositories;
using LaunchBox.Core.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;

namespace LaunchBox.Windows;

public partial class EmulatorManagerWindow : Window
{
    private readonly IEmulatorService _emulatorService;
    private readonly IEmulatorDetectionService _detectionService;
    private readonly IRepository<Platform> _platformRepository;
    private readonly ObservableCollection<Emulator> _emulators = new();
    private IEnumerable<Platform> _platforms = Enumerable.Empty<Platform>();

    public EmulatorManagerWindow(
        IEmulatorService emulatorService,
        IEmulatorDetectionService detectionService,
        IRepository<Platform> platformRepository)
    {
        InitializeComponent();
        _emulatorService = emulatorService;
        _detectionService = detectionService;
        _platformRepository = platformRepository;

        EmulatorListBox.ItemsSource = _emulators;
        Loaded += EmulatorManagerWindow_Loaded;
    }

    private async void EmulatorManagerWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        // Load platforms
        _platforms = await _platformRepository.GetAllAsync();
        PlatformComboBox.ItemsSource = _platforms;

        // Load emulators
        var emulators = await _emulatorService.GetAllEmulatorsAsync();
        _emulators.Clear();
        foreach (var emulator in emulators)
        {
            _emulators.Add(emulator);
        }
    }

    private void EmulatorListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (EmulatorListBox.SelectedItem is Emulator emulator)
        {
            LoadEmulatorDetails(emulator);
        }
    }

    private void LoadEmulatorDetails(Emulator emulator)
    {
        NameTextBox.Text = emulator.Name;
        ExecutablePathTextBox.Text = emulator.ExecutablePath;
        ArgumentsTextBox.Text = emulator.CommandLineArguments;
        WorkingDirectoryTextBox.Text = emulator.WorkingDirectory;
        UseQuotesCheckBox.IsChecked = emulator.UseQuotesForPath;
        IsDefaultCheckBox.IsChecked = emulator.IsDefault;

        if (emulator.PlatformId.HasValue)
        {
            PlatformComboBox.SelectedItem = _platforms.FirstOrDefault(p => p.Id == emulator.PlatformId);
        }
        else
        {
            PlatformComboBox.SelectedIndex = -1;
        }
    }

    private void AddEmulator_Click(object sender, RoutedEventArgs e)
    {
        var newEmulator = new Emulator
        {
            Name = "New Emulator",
            ExecutablePath = "",
            UseQuotesForPath = true
        };

        _emulators.Add(newEmulator);
        EmulatorListBox.SelectedItem = newEmulator;
    }

    private async void AutoDetect_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as System.Windows.Controls.Button;
        if (button != null) button.IsEnabled = false;

        try
        {
            var detected = await _detectionService.ScanForEmulatorsAsync();
            var count = 0;

            foreach (var emulator in detected)
            {
                // Check if already exists
                if (!_emulators.Any(e => e.ExecutablePath.Equals(emulator.ExecutablePath, StringComparison.OrdinalIgnoreCase)))
                {
                    await _emulatorService.AddEmulatorAsync(emulator);
                    _emulators.Add(emulator);
                    count++;
                }
            }

            MessageBox.Show($"Found and added {count} emulator(s)", "Auto-Detection Complete",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error during auto-detection: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            if (button != null) button.IsEnabled = true;
        }
    }

    private async void DeleteEmulator_Click(object sender, RoutedEventArgs e)
    {
        if (EmulatorListBox.SelectedItem is Emulator emulator)
        {
            var result = MessageBox.Show($"Delete emulator '{emulator.Name}'?", "Confirm Delete",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (emulator.Id > 0)
                {
                    await _emulatorService.DeleteEmulatorAsync(emulator.Id);
                }
                _emulators.Remove(emulator);
            }
        }
    }

    private void BrowseExecutable_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select Emulator Executable",
            Filter = "Executable Files|*.exe|All Files|*.*"
        };

        if (dialog.ShowDialog() == true)
        {
            ExecutablePathTextBox.Text = dialog.FileName;
        }
    }

    private async void SaveEmulator_Click(object sender, RoutedEventArgs e)
    {
        if (EmulatorListBox.SelectedItem is not Emulator emulator) return;

        emulator.Name = NameTextBox.Text;
        emulator.ExecutablePath = ExecutablePathTextBox.Text;
        emulator.CommandLineArguments = ArgumentsTextBox.Text;
        emulator.WorkingDirectory = WorkingDirectoryTextBox.Text;
        emulator.UseQuotesForPath = UseQuotesCheckBox.IsChecked == true;
        emulator.IsDefault = IsDefaultCheckBox.IsChecked == true;
        emulator.PlatformId = (PlatformComboBox.SelectedItem as Platform)?.Id;

        try
        {
            if (emulator.Id == 0)
            {
                await _emulatorService.AddEmulatorAsync(emulator);
            }
            else
            {
                await _emulatorService.UpdateEmulatorAsync(emulator);
            }

            MessageBox.Show("Emulator saved successfully!", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to save emulator: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void TestEmulator_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Emulator testing not yet implemented.\n\nYou can test by launching a game.",
            "Test Emulator", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private async void ValidateEmulator_Click(object sender, RoutedEventArgs e)
    {
        if (EmulatorListBox.SelectedItem is not Emulator emulator) return;

        // Update from UI
        emulator.ExecutablePath = ExecutablePathTextBox.Text;
        emulator.WorkingDirectory = WorkingDirectoryTextBox.Text;

        var result = await _detectionService.ValidateEmulatorAsync(emulator);

        ValidationPanel.Visibility = Visibility.Visible;

        if (result.IsValid)
        {
            ValidationText.Text = $"✓ Emulator is valid\nVersion: {result.Version ?? "Unknown"}";
            ValidationPanel.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 100, 0));
        }
        else
        {
            ValidationText.Text = "✗ Validation failed:\n" + string.Join("\n", result.Issues);
            ValidationPanel.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(139, 0, 0));
        }
    }
}
