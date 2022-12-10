using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Unchase.OpenAPI.ConnectedService.Converters
{
    public class EqualityConverter :
        IValueConverter
    {
        public object Convert(
            object valueObject,
            Type targetType,
            object parameterObject,
            CultureInfo culture)
        {
            var str1 = parameterObject != null
                ? parameterObject.ToString()
                : string.Empty;
            var str2 = valueObject != null
                ? valueObject.ToString()
                : string.Empty;

            bool flag;
            if (str1.StartsWith("!"))
            {
                flag = str1.Substring(1) != str2;
            }
            else
            {
                flag = str1.Split(',').Contains(str2);
            }

            if (targetType == typeof(Visibility))
            {
                return (Visibility)(flag ? 0 : 2);
            }

            return flag;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}