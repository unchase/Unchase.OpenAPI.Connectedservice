using System.Windows;
using System.Windows.Controls;

namespace Unchase.OpenAPI.ConnectedService.Views
{
    public partial class TypeScriptClientSettings : UserControl
    {
        public TypeScriptClientSettings()
        {
            InitializeComponent();
        }

        private void OpenExtensionCodeFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".ts",
                Filter = "TypeScript Files (.ts)|*.ts",
                Title = "Open Class extension code file"
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                var filename = openFileDialog.FileName;
                ExtensionCodeFileTextBox.Text = filename;
            }
        }
    }
}