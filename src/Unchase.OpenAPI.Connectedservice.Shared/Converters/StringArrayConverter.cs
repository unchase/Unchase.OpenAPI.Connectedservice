using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Unchase.OpenAPI.ConnectedService.Converters
{
    public class StringArrayConverter :
        IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            var separator = "\n";
            if (parameter != null)
            {
                separator = (string)parameter;
            }

            return value != null
                ? string.Join(separator, (string[])value)
                : string.Empty;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            var separator = '\n';
            if (parameter != null)
            {
                separator = System.Convert.ToChar(parameter);
            }

            return value?.ToString()
                       .Trim('\r')
                       .Split(separator)
                       .Select(s => s.Trim())
                       .Where(n => !string.IsNullOrEmpty(n))
                       .ToArray() ?? new string[] { };
        }
    }
}