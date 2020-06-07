using System;
using System.Windows.Data;

namespace DotNetMonitor.UI.Converters
{
    public class SearchValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string cellText = values[0] == null ? string.Empty : values[0].ToString();
            string searchText = values[1] as string;

            if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(cellText))
            {
                return cellText.ToLower().StartsWith(searchText.ToLower());
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}