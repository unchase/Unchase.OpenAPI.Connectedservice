using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Unchase.OpenAPI.ConnectedService.Converters
{
    public class NotConverter :
        IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (targetType == typeof(Visibility))
            {
                return (Visibility)((Visibility)new VisibilityConverter().Convert(value, targetType, parameter, culture) == Visibility.Visible ? 2 : 0);
            }

            if (!(targetType == typeof(bool)))
            {
                return null;
            }

            if (value == null)
            {
                return true;
            }

            return !(bool)value;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is bool && targetType == typeof(bool))
                return !(bool)value;
            throw new NotImplementedException();
        }
    }
}