using System.Windows;
using System.Windows.Controls;

namespace Unchase.OpenAPI.ConnectedService.Common
{
    internal static class ExtensionsHelper
    {
        internal static void ChangeStackPanelVisibility(this StackPanel stackPanel)
        {
            if (stackPanel.Visibility == Visibility.Collapsed)
            {
                stackPanel.Visibility = Visibility.Visible;
            }
            else if (stackPanel.Visibility == Visibility.Visible)
            {
                stackPanel.Visibility = Visibility.Collapsed;
            }
        }

        internal static bool TryDowncastToFEorFCE(this DependencyObject d, out FrameworkElement fe, out FrameworkContentElement fce)
        {
            fe = null;
            fce = null;
            if (d is FrameworkElement fe2)
            {
                fe = fe2;
                return true;
            }
            else if (d is FrameworkContentElement fce2)
            {
                fce = fce2;
                return true;
            }
            return false;
        }
    }
}