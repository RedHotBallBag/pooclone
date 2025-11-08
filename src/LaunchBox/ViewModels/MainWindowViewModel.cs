using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LaunchBox.Core.Models;
using LaunchBox.Core.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace LaunchBox.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IGameLibraryService _gameLibraryService;
    private readonly IGameLauncherService _gameLauncherService;
    private readonly IEmulatorService _emulatorService;

    [ObservableProperty]
    private ObservableCollection<Game> _games = new();

    [ObservableProperty]
    private ObservableCollection<Platform> _platforms = new();

    [ObservableProperty]
    private Game? _selectedGame;

    [ObservableProperty]
    private Platform? _selectedPlatform;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _showFavoritesOnly;

    [ObservableProperty]
    private ViewMode _currentViewMode = ViewMode.Grid;

    [ObservableProperty]
    private bool _isLoading;

    public MainWindowViewModel(
        IGameLibraryService gameLibraryService,
        IGameLauncherService gameLauncherService,
        IEmulatorService emulatorService)
    {
        _gameLibraryService = gameLibraryService;
        _gameLauncherService = gameLauncherService;
        _emulatorService = emulatorService;
    }

    public async Task InitializeAsync()
    {
        await LoadGamesAsync();
    }

    [RelayCommand]
    private async Task LoadGamesAsync()
    {
        IsLoading = true;
        try
        {
            IEnumerable<Game> games;

            if (ShowFavoritesOnly)
            {
                games = await _gameLibraryService.GetFavoriteGamesAsync();
            }
            else if (SelectedPlatform != null)
            {
                games = await _gameLibraryService.GetGamesByPlatformAsync(SelectedPlatform.Id);
            }
            else if (!string.IsNullOrWhiteSpace(SearchText))
            {
                games = await _gameLibraryService.SearchGamesAsync(SearchText);
            }
            else
            {
                games = await _gameLibraryService.GetAllGamesAsync();
            }

            Games.Clear();
            foreach (var game in games)
            {
                Games.Add(game);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load games: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task LaunchGameAsync(Game? game)
    {
        if (game == null)
            return;

        try
        {
            await _gameLauncherService.LaunchGameAsync(game);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to launch game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync(Game? game)
    {
        if (game == null)
            return;

        try
        {
            await _gameLibraryService.ToggleFavoriteAsync(game.Id);
            await LoadGamesAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to toggle favorite: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await LoadGamesAsync();
    }

    [RelayCommand]
    private async Task FilterByPlatformAsync(Platform? platform)
    {
        SelectedPlatform = platform;
        await LoadGamesAsync();
    }

    [RelayCommand]
    private async Task ToggleShowFavoritesAsync()
    {
        ShowFavoritesOnly = !ShowFavoritesOnly;
        await LoadGamesAsync();
    }

    [RelayCommand]
    private void SwitchViewMode(string mode)
    {
        CurrentViewMode = mode switch
        {
            "Grid" => ViewMode.Grid,
            "List" => ViewMode.List,
            "Details" => ViewMode.Details,
            _ => ViewMode.Grid
        };
    }

    [RelayCommand]
    private async Task AddGameAsync()
    {
        // TODO: Show add game dialog
        MessageBox.Show("Add game dialog - to be implemented", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private async Task ScanRomFolderAsync()
    {
        // TODO: Show scan folder dialog
        MessageBox.Show("Scan ROM folder dialog - to be implemented", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void OpenSettings()
    {
        // TODO: Show settings window
        MessageBox.Show("Settings window - to be implemented", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    partial void OnSearchTextChanged(string value)
    {
        // Debounce search
        Task.Run(async () =>
        {
            await Task.Delay(300);
            await LoadGamesAsync();
        });
    }
}
