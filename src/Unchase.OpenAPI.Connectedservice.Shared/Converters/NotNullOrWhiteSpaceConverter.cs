using System;
using System.Globalization;
using System.Windows.Data;

namespace Unchase.OpenAPI.ConnectedService.Converters
{
    class NotNullOrWhiteSpaceConverter :
        IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (targetType == typeof(string))
            {
                if (string.IsNullOrWhiteSpace(value as string) && parameter is string defaultValue)
                {
                    return defaultValue;
                }

                return value;
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (targetType == typeof(string))
            {
                if (string.IsNullOrWhiteSpace(value as string) && parameter is string defaultValue)
                {
                    return defaultValue;
                }

                return value;
            }

            throw new NotImplementedException();
        }
    }
}