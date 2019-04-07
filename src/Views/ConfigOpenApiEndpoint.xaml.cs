using System.Windows;
using System.Windows.Controls;

namespace Unchase.OpenAPI.ConnectedService.Views
{
    public partial class ConfigOpenApiEndpoint : UserControl
    {
        private readonly Wizard _wizard;

        internal ConfigOpenApiEndpoint(Wizard wizard)
        {
            InitializeComponent();
            _wizard = wizard;
        }

        #region Events
        private void GenerateCSharpClient_OnUnchecked(object sender, RoutedEventArgs e)
        {
            this._wizard.RemoveCSharpClientSettingsPage();
        }

        private void GenerateCSharpClient_OnChecked(object sender, RoutedEventArgs e)
        {
            this._wizard.AddCSharpClientSettingsPage();
        }

        private void GenerateCSharpController_OnChecked(object sender, RoutedEventArgs e)
        {
            this._wizard.AddCSharpControllerSettingsPage();
        }

        private void GenerateCSharpController_OnUnchecked(object sender, RoutedEventArgs e)
        {
            this._wizard.RemoveCSharpControllerSettingsPage();
        }

        private void GenerateTypeScriptClient_OnChecked(object sender, RoutedEventArgs e)
        {
            this._wizard.AddTypeScriptClientSettingsPage();
        }

        private void GenerateTypeScriptClient_OnUnchecked(object sender, RoutedEventArgs e)
        {
            this._wizard.RemoveTypeScriptClientSettingsPage();
        }
        #endregion
    }
}
