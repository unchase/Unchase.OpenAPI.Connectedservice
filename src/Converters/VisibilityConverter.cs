using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace Unchase.OpenAPI.ConnectedService.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = value != null;
            if (value is bool)
                flag = (bool)value;
            if (value is string)
                flag = !string.IsNullOrEmpty((string)value);
            if (value is IList)
            {
                IList list = (IList)value;
                if (list.Count == 0)
                {
                    flag = false;
                }
                else
                {
                    flag = true;
                    if (parameter is string)
                    {
                        string str = parameter.ToString();
                        if (str.StartsWith("CheckAll:"))
                        {
                            string name = str.Substring(9);
                            foreach (object obj in (IEnumerable)list)
                            {
                                PropertyInfo property = obj.GetType().GetProperty(name);
                                if (property != (PropertyInfo)null && !(bool)this.Convert(property.GetValue(obj, (object[])null), typeof(bool), (object)null, (CultureInfo)null))
                                {
                                    flag = false;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (value is int)
                flag = (int)value > 0;
            if (value is Visibility)
                flag = (Visibility)value == Visibility.Visible;
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
