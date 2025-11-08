using LaunchBox.Core.Models;

namespace LaunchBox.Core.Services;

public interface IRomScannerService
{
    Task<IEnumerable<Game>> ScanFolderAsync(string folderPath, int platformId);
    IEnumerable<string> GetSupportedExtensions(int platformId);
}
