using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Unchase.OpenAPI.ConnectedService.Converters
{
    class VisibilityToHyperlinkTextConverter :
        IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                switch (visibility)
                {
                    case Visibility.Visible:
                    {
                        return "Hide";
                    }

                    default:
                        return "Show";
                }
            }

            return "Show";
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