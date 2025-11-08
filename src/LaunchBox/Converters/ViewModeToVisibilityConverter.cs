using LaunchBox.Core.Models;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LaunchBox.Converters;

public class ViewModeToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ViewMode currentMode && parameter is string targetMode)
        {
            bool matches = currentMode.ToString() == targetMode;
            return matches ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
