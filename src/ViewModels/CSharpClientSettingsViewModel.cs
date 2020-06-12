using System;
using System.Linq;
using Microsoft.VisualStudio.ConnectedServices;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag.Commands;
using NSwag.Commands.CodeGeneration;
using Unchase.OpenAPI.ConnectedService.Views;

namespace Unchase.OpenAPI.ConnectedService.ViewModels
{
    internal class CSharpClientSettingsViewModel : ConnectedServiceWizardPage
    {
        #region Properties
        public string GeneratedFileName { get; set; }

        public OpenApiToCSharpClientCommand Command { get; set; } = new OpenApiToCSharpClientCommand
        {
            Namespace = string.Empty,
            OperationGenerationMode = OperationGenerationMode.SingleClientFromPathSegments
        };

        /// <summary>Gets the list of operation modes. </summary>
        public OperationGenerationMode[] OperationGenerationModes { get; } = Enum.GetNames(typeof(OperationGenerationMode))
            .Select(t => (OperationGenerationMode)Enum.Parse(typeof(OperationGenerationMode), t))
            .ToArray();

        /// <summary>Gets the list of class styles. </summary>
        public CSharpClassStyle[] ClassStyles { get; } = Enum.GetNames(typeof(CSharpClassStyle))
            .Select(t => (CSharpClassStyle)Enum.Parse(typeof(CSharpClassStyle), t))
            .ToArray();

        /// <summary>Gets new line behaviors. </summary>
        public NewLineBehavior[] NewLineBehaviors { get; } = Enum.GetNames(typeof(NewLineBehavior))
            .Select(t => (NewLineBehavior)Enum.Parse(typeof(NewLineBehavior), t))
            .ToArray();

        public bool ExcludeTypeNamesLater { get; set; }
        #endregion

        #region Constructors
        public CSharpClientSettingsViewModel() : base()
        {
            this.Title = "CSharp Client Settings";
            this.Description = "Settings for generating CSharp client";
            this.Legend = "CSharp Client Settings";
            this.View = new CSharpClientSettings {DataContext = this};
        }
        #endregion
    }
}
