using System.Globalization;
using System.Windows.Data;

namespace LaunchBox.Converters;

public class FavoriteButtonTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isFavorite)
        {
            return isFavorite ? "★ Remove from Favorites" : "☆ Add to Favorites";
        }
        return "☆ Add to Favorites";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
