using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Unchase.OpenAPI.ConnectedService.Converters
{
    public class EqualityConverter : IValueConverter
    {
        public object Convert(
            object valueObject,
            Type targetType,
            object parameterObject,
            CultureInfo culture)
        {
            string str1 = parameterObject != null ? parameterObject.ToString() : string.Empty;
            string str2 = valueObject != null ? valueObject.ToString() : string.Empty;
            bool flag;
            if (str1.StartsWith("!"))
                flag = str1.Substring(1) != str2;
            else
                flag = ((IEnumerable<string>)str1.Split(',')).Contains<string>(str2);
            if (targetType == typeof(Visibility))
                return (object)(Visibility)(flag ? 0 : 2);
            return (object)flag;
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
