# LaunchBox - Game Library Manager MVP

A modern, desktop-first game library management application built with WPF and .NET 8. LaunchBox simplifies and beautifies game library management across multiple emulators.

## Features

### Core MVP Features

#### 1. Game Library Management
- Add games manually or via ROM folder scanning
- Basic metadata parsing: Title, platform, release year
- Platform categorization: NES, SNES, PS1, N64, and more
- Tagging and filtering: Genre, favorites, recently played
- Search functionality across titles, developers, and publishers

#### 2. Emulator Integration
- Add and configure emulators with custom paths and command-line arguments
- Launch games directly via configured emulators
- Built-in presets for popular emulators:
  - RetroArch (multiple cores)
  - Project64
  - Dolphin
  - PCSX2
  - ePSXe
  - DeSmuME
  - And more...

#### 3. Media Display
- Box art, screenshots, and background images
- Grid and list view modes
- Detailed game information panel
- Visual favorites system

#### 4. User Experience
- Modern dark theme interface
- Responsive grid-based layout
- Real-time search with debouncing
- Platform filtering
- Play count and last played tracking

#### 5. Settings and Configuration
- SQLite database for local storage
- Automatic database migrations
- Configurable media folders
- Emulator preset initialization

## Technical Architecture

### Technology Stack
- **Frontend**: WPF (Windows Presentation Foundation) with .NET 8
- **Backend**: Entity Framework Core with SQLite
- **Pattern**: MVVM (Model-View-ViewModel) with CommunityToolkit.Mvvm
- **DI**: Microsoft.Extensions.DependencyInjection
- **Logging**: Serilog

### Project Structure
```
LaunchBox/
├── src/
│   ├── LaunchBox/              # WPF Application
│   │   ├── ViewModels/         # MVVM ViewModels
│   │   ├── Converters/         # XAML Value Converters
│   │   ├── Styles/             # XAML Styles and Themes
│   │   └── App.xaml            # Application Entry Point
│   ├── LaunchBox.Core/         # Domain Models and Services
│   │   ├── Models/             # Entity Models
│   │   └── Services/           # Business Logic Services
│   └── LaunchBox.Data/         # Data Access Layer
│       ├── Repositories/       # Repository Pattern Implementation
│       └── LaunchBoxDbContext.cs
└── LaunchBox.sln
```

### Key Components

#### Models
- **Game**: Represents a game with metadata, media paths, and play statistics
- **Platform**: Gaming platforms (NES, SNES, PlayStation, etc.)
- **Emulator**: Emulator configurations with executable paths and arguments
- **Tag**: Custom tags for organizing games
- **AppSettings**: User preferences and application settings

#### Services
- **GameLibraryService**: Core game management operations
- **RomScannerService**: Automated ROM folder scanning
- **EmulatorService**: Emulator configuration management
- **GameLauncherService**: Game launching logic
- **MediaScraperService**: Future media downloading (placeholder for MVP)

#### Repositories
- Generic repository pattern with specialized implementations
- **GameRepository**: Advanced queries for games (favorites, recent, search)
- Support for eager loading with Entity Framework Include

## Getting Started

### Prerequisites
- Windows 10/11
- .NET 8 SDK
- Visual Studio 2022 or later (recommended)

### Building the Application

1. Clone the repository:
```bash
git clone <repository-url>
cd pooclone
```

2. Restore NuGet packages:
```bash
dotnet restore
```

3. Build the solution:
```bash
dotnet build
```

4. Run the application:
```bash
dotnet run --project src/LaunchBox/LaunchBox.csproj
```

### First-Time Setup

1. **Configure Emulators**: The application comes with preset configurations, but you'll need to update the executable paths to match your system
2. **Scan ROM Folder**: Use the "Scan ROM Folder" feature to automatically import your games
3. **Add Games Manually**: Use "Add Game" to manually add individual titles

## Database

The application uses SQLite for data storage. The database is automatically created at:
```
%APPDATA%\LaunchBox\launchbox.db
```

Migrations are applied automatically on first run.

## Supported Platforms (Pre-configured)

1. Nintendo Entertainment System (NES)
2. Super Nintendo Entertainment System (SNES)
3. Nintendo 64 (N64)
4. PlayStation (PS1)
5. PlayStation 2 (PS2)
6. Sega Genesis
7. Game Boy / Game Boy Color
8. Game Boy Advance
9. Nintendo DS
10. Arcade

## Supported ROM File Extensions

The ROM scanner recognizes the following file extensions by platform:

- **NES**: .nes, .unif, .fds
- **SNES**: .sfc, .smc
- **N64**: .z64, .n64, .v64
- **PlayStation**: .bin, .cue, .iso, .img
- **Genesis**: .md, .bin, .gen
- **Game Boy**: .gb, .gbc
- **GBA**: .gba
- **Nintendo DS**: .nds
- **Arcade**: .zip

## Future Enhancements (Post-MVP)

- Media Pack Browser integration
- Plugin architecture for community extensions
- Cloud synchronization
- Automated metadata scraping from online databases (IGDB, TheGamesDB)
- Big Box mode for full-screen couch gaming
- Controller navigation
- Video preview support
- Achievement tracking
- Multi-language support

## Contributing

This is an MVP implementation. Contributions are welcome!

## License

[Specify your license here]

## Acknowledgments

Built as an MVP demonstration of:
- Clean architecture principles
- MVVM pattern in WPF
- Repository pattern with Entity Framework Core
- Service-oriented design
- Modern UI/UX for desktop applications
