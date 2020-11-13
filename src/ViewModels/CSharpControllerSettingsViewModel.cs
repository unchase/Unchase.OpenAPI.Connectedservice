using System;
using System.Linq;
using Microsoft.VisualStudio.ConnectedServices;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag.CodeGeneration.CSharp.Models;
using NSwag.Commands;
using NSwag.Commands.CodeGeneration;
using Unchase.OpenAPI.ConnectedService.Views;

namespace Unchase.OpenAPI.ConnectedService.ViewModels
{
    internal class CSharpControllerSettingsViewModel : ConnectedServiceWizardPage
    {
        #region Properties

        public OpenApiToCSharpControllerCommand Command { get; set; } = new OpenApiToCSharpControllerCommand { Namespace = string.Empty };

        /// <summary>Gets the list of operation modes.</summary>
        public OperationGenerationMode[] OperationGenerationModes { get; } = Enum.GetNames(typeof(OperationGenerationMode))
            .Select(t => (OperationGenerationMode)Enum.Parse(typeof(OperationGenerationMode), t))
            .ToArray();

        /// <summary>Gets the list of class styles.</summary>
        public CSharpClassStyle[] ClassStyles { get; } = Enum.GetNames(typeof(CSharpClassStyle))
            .Select(t => (CSharpClassStyle)Enum.Parse(typeof(CSharpClassStyle), t))
            .ToArray();

        /// <summary>Gets the list of class styles.</summary>
        public CSharpControllerStyle[] ControllerStyles { get; } = Enum.GetNames(typeof(CSharpControllerStyle))
            .Select(t => (CSharpControllerStyle)Enum.Parse(typeof(CSharpControllerStyle), t))
            .ToArray();

        /// <summary>Gets the list of class targets.</summary>
        public CSharpControllerTarget[] ControllerTargets { get; } = Enum.GetNames(typeof(CSharpControllerTarget))
            .Select(t => (CSharpControllerTarget)Enum.Parse(typeof(CSharpControllerTarget), t))
            .ToArray();

        /// <summary>Gets the list of route naming strategies.</summary>
        public CSharpControllerRouteNamingStrategy[] RouteNamingStrategies { get; } = Enum.GetNames(typeof(CSharpControllerRouteNamingStrategy))
            .Select(t => (CSharpControllerRouteNamingStrategy)Enum.Parse(typeof(CSharpControllerRouteNamingStrategy), t))
            .ToArray();

        /// <summary>Gets new line behaviors. </summary>
        public NewLineBehavior[] NewLineBehaviors { get; } = Enum.GetNames(typeof(NewLineBehavior))
            .Select(t => (NewLineBehavior)Enum.Parse(typeof(NewLineBehavior), t))
            .ToArray();

        #endregion

        #region Constructors

        public CSharpControllerSettingsViewModel() : base()
        {
            this.Title = "CSharp Controller Settings";
            this.Description = "Settings for generating CSharp controller";
            this.Legend = "CSharp Controller Settings";
            this.View = new CSharpControllerSettings {DataContext = this};
        }

        #endregion
    }
}
