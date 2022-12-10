using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Unchase.OpenAPI.ConnectedService.Converters
{
    public class VisibilityConverter :
        IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            var flag = value != null;

            if (value is bool b)
            {
                flag = b;
            }

            if (value is string s)
            {
                flag = !string.IsNullOrEmpty(s);
            }

            if (value is IList list)
            {
                if (list.Count == 0)
                {
                    flag = false;
                }
                else
                {
                    if (parameter is string)
                    {
                        var str = parameter.ToString();
                        if (str.StartsWith("CheckAll:"))
                        {
                            var name = str.Substring(9);
                            foreach (var obj in list)
                            {
                                var property = obj.GetType().GetProperty(name);
                                if (property != null && !(bool)Convert(property.GetValue(obj, null), typeof(bool), null, null))
                                {
                                    flag = false;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (value is int i)
            {
                flag = i > 0;
            }

            if (value is Visibility visibility)
            {
                flag = visibility == Visibility.Visible;
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