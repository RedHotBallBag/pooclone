namespace LaunchBox.Core.Services;

/// <summary>
/// Service for auto-detecting game platforms from file paths
/// </summary>
public interface IPlatformDetectionService
{
    /// <summary>
    /// Detect platform from file extension and path
    /// </summary>
    int? DetectPlatformFromFile(string filePath);

    /// <summary>
    /// Detect platform from folder name
    /// </summary>
    int? DetectPlatformFromFolder(string folderPath);

    /// <summary>
    /// Get supported extensions for a platform
    /// </summary>
    IEnumerable<string> GetPlatformExtensions(int platformId);
}
