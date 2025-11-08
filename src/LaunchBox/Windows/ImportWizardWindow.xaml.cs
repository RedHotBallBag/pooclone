using LaunchBox.Core.Models;
using LaunchBox.Core.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace LaunchBox.Windows;

public partial class ImportWizardWindow : Window
{
    private readonly IGameLibraryService _gameLibrary;
    private readonly IPlatformDetectionService _platformDetection;
    private readonly IEnumerable<Platform> _platforms;
    private readonly ObservableCollection<ImportFileItem> _importFiles = new();

    public ImportWizardWindow(
        IGameLibraryService gameLibrary,
        IPlatformDetectionService platformDetection,
        IEnumerable<Platform> platforms)
    {
        InitializeComponent();
        _gameLibrary = gameLibrary;
        _platformDetection = platformDetection;
        _platforms = platforms;

        DefaultPlatformComboBox.ItemsSource = _platforms;
        FilesListBox.ItemsSource = _importFiles;
    }

    private void ImportArea_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Copy;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
        e.Handled = true;
    }

    private void ImportArea_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            ProcessFiles(files);
        }
    }

    private void BrowseFiles_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select ROM Files",
            Filter = "All ROM Files|*.nes;*.sfc;*.smc;*.z64;*.n64;*.cue;*.iso;*.md;*.gen;*.gb;*.gbc;*.gba;*.nds;*.zip|All Files|*.*",
            Multiselect = true
        };

        if (dialog.ShowDialog() == true)
        {
            ProcessFiles(dialog.FileNames);
        }
    }

    private void BrowseFolder_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new System.Windows.Forms.FolderBrowserDialog
        {
            Description = "Select ROM Folder"
        };

        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            var files = Directory.GetFiles(dialog.SelectedPath, "*.*", SearchOption.AllDirectories)
                .Where(f => IsRomFile(f))
                .ToArray();
            ProcessFiles(files);
        }
    }

    private void ProcessFiles(string[] filePaths)
    {
        DropInstructions.Visibility = Visibility.Collapsed;

        foreach (var filePath in filePaths)
        {
            if (!IsRomFile(filePath)) continue;

            var fileName = Path.GetFileName(filePath);
            Platform? detectedPlatform = null;

            if (AutoDetectPlatformCheckBox.IsChecked == true)
            {
                var platformId = _platformDetection.DetectPlatformFromFile(filePath);
                if (platformId.HasValue)
                {
                    detectedPlatform = _platforms.FirstOrDefault(p => p.Id == platformId.Value);
                }
            }

            var defaultPlatform = detectedPlatform ?? (DefaultPlatformComboBox.SelectedItem as Platform);

            var item = new ImportFileItem
            {
                FileName = fileName,
                FilePath = filePath,
                Platforms = new ObservableCollection<Platform>(_platforms),
                SelectedPlatform = defaultPlatform,
                Status = "Ready"
            };

            _importFiles.Add(item);
        }

        ImportButton.IsEnabled = _importFiles.Any();
        StatusText.Text = $"{_importFiles.Count} file(s) ready to import";
    }

    private bool IsRomFile(string filePath)
    {
        var ext = Path.GetExtension(filePath).ToLower();
        var validExtensions = new[] { ".nes", ".sfc", ".smc", ".z64", ".n64", ".v64", ".cue", ".iso", ".bin",
            ".md", ".gen", ".smd", ".gb", ".gbc", ".gba", ".nds", ".zip", ".7z", ".chd", ".pbp" };
        return validExtensions.Contains(ext);
    }

    private async void Import_Click(object sender, RoutedEventArgs e)
    {
        ImportButton.IsEnabled = false;
        var imported = 0;

        foreach (var item in _importFiles)
        {
            if (item.SelectedPlatform == null) continue;

            try
            {
                var game = new Game
                {
                    Title = CleanGameTitle(item.FileName),
                    FilePath = item.FilePath,
                    PlatformId = item.SelectedPlatform.Id,
                    CreatedAt = DateTime.UtcNow
                };

                await _gameLibrary.AddGameAsync(game);
                item.Status = "✓ Imported";
                imported++;
            }
            catch (Exception ex)
            {
                item.Status = "✗ Failed";
            }
        }

        StatusText.Text = $"Imported {imported} of {_importFiles.Count} games";
        MessageBox.Show($"Successfully imported {imported} games!", "Import Complete",
            MessageBoxButton.OK, MessageBoxImage.Information);

        DialogResult = true;
        Close();
    }

    private string CleanGameTitle(string fileName)
    {
        var title = Path.GetFileNameWithoutExtension(fileName);
        title = System.Text.RegularExpressions.Regex.Replace(title, @"\([^)]*\)", "");
        title = System.Text.RegularExpressions.Regex.Replace(title, @"\[[^\]]*\]", "");
        title = System.Text.RegularExpressions.Regex.Replace(title, @"\s+", " ").Trim();
        return title;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    public class ImportFileItem
    {
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public ObservableCollection<Platform> Platforms { get; set; } = new();
        public Platform? SelectedPlatform { get; set; }
        public string Status { get; set; } = "Ready";
    }
}
