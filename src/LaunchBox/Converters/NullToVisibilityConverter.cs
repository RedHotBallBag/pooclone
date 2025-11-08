using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LaunchBox.Converters;

public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool inverse = parameter?.ToString() == "Inverse";
        bool isNull = value == null;

        if (inverse)
        {
            return isNull ? Visibility.Collapsed : Visibility.Visible;
        }
        else
        {
            return isNull ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
