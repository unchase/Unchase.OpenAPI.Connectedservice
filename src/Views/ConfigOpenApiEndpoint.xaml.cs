﻿using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Unchase.OpenAPI.ConnectedService.ViewModels;

namespace Unchase.OpenAPI.ConnectedService.Views
{
    public partial class ConfigOpenApiEndpoint : UserControl
    {
        #region Properties and Fields
        private readonly Wizard _wizard;

        private const string ReportABugUrlFormat = "https://github.com/unchase/Unchase.OpenAPI.Connectedservice/issues/new?title={0}&labels=bug&body={1}";
        #endregion

        #region Constructors
        internal ConfigOpenApiEndpoint(Wizard wizard)
        {
            InitializeComponent();
            this._wizard = wizard;
        }
        #endregion

        #region Methods

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

        private void ReportABugButton_Click(object sender, RoutedEventArgs e)
        {
            var title = Uri.EscapeUriString("<BUG title>");
            var body = Uri.EscapeUriString("<Please describe what bug you found when using the service.>");
            var url = string.Format(ReportABugUrlFormat, title, body);
            System.Diagnostics.Process.Start(url);
        }

        private void OpenEndpointFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = (this.DataContext as ConfigOpenApiEndpointViewModel)?.UserSettings?.ConvertFromOdata == true ? ".xml" : ".json",
                Filter = "Specification Files (.*)|*.*",
                Title = "Open specification file"
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                if (this._wizard.ConfigOpenApiEndpointViewModel.UserSettings.UseRelativePath && openFileDialog.FileName.StartsWith(this._wizard.ProjectPath, StringComparison.OrdinalIgnoreCase))
                    Endpoint.Text = openFileDialog.FileName.Substring(this._wizard.ProjectPath.Length);
                else
                    Endpoint.Text = openFileDialog.FileName;
            }
        }

        private void NumericTexBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        #endregion
    }
}
