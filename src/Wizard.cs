//-----------------------------------------------------------------------
// <copyright file="Wizard.cs" company="Unchase">
//     Copyright (c) Nikolay Chebotov (Unchase). All rights reserved.
// </copyright>
// <license>https://github.com/unchase/Unchase.OpenAPI.Connectedservice/blob/master/LICENSE.md</license>
// <author>Nickolay Chebotov (Unchase), spiritkola@hotmail.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.VisualStudio.ConnectedServices;
using Unchase.OpenAPI.ConnectedService.Models;
using Unchase.OpenAPI.ConnectedService.ViewModels;
using Unchase.OpenAPI.ConnectedService.Views;

namespace Unchase.OpenAPI.ConnectedService
{
    internal class Wizard : ConnectedServiceWizard
    {
        private Instance _serviceInstance;

        public ConfigOpenApiEndpointViewModel ConfigOpenApiEndpointViewModel { get; set; }

        public CSharpClientSettingsViewModel CSharpClientSettingsViewModel { get; set; }

        public TypeScriptClientSettingsViewModel TypeScriptClientSettingsViewModel { get; set; }

        public CSharpControllerSettingsViewModel CSharpControllerSettingsViewModel { get; set; }

        public ConnectedServiceProviderContext Context { get; set; }

        public Instance ServiceInstance => this._serviceInstance ?? (this._serviceInstance = new Instance());

        public UserSettings UserSettings { get; }

        public Wizard(ConnectedServiceProviderContext context)
        {
            this.Context = context;
            this.UserSettings = UserSettings.Load(context.Logger);

            ConfigOpenApiEndpointViewModel = new ConfigOpenApiEndpointViewModel(this.UserSettings, this);
            CSharpClientSettingsViewModel = new CSharpClientSettingsViewModel();
            TypeScriptClientSettingsViewModel = new TypeScriptClientSettingsViewModel();
            CSharpControllerSettingsViewModel = new CSharpControllerSettingsViewModel();

            if (this.Context.IsUpdating)
            {
                var serviceConfig = this.Context.GetExtendedDesignerData<ServiceConfiguration>();
                ConfigOpenApiEndpointViewModel.Endpoint = serviceConfig.Endpoint;
                ConfigOpenApiEndpointViewModel.ServiceName = serviceConfig.ServiceName;
                ConfigOpenApiEndpointViewModel.UseWebProxy = serviceConfig.UseWebProxy;
                ConfigOpenApiEndpointViewModel.NetworkCredentialsDomain = serviceConfig.NetworkCredentialsDomain;
                ConfigOpenApiEndpointViewModel.NetworkCredentialsUserName = serviceConfig.NetworkCredentialsUserName;
                ConfigOpenApiEndpointViewModel.NetworkCredentialsPassword = serviceConfig.NetworkCredentialsPassword;
                ConfigOpenApiEndpointViewModel.WebProxyNetworkCredentialsDomain = serviceConfig.WebProxyNetworkCredentialsDomain;
                ConfigOpenApiEndpointViewModel.WebProxyNetworkCredentialsUserName = serviceConfig.WebProxyNetworkCredentialsUserName;
                ConfigOpenApiEndpointViewModel.WebProxyNetworkCredentialsPassword = serviceConfig.WebProxyNetworkCredentialsPassword;
                ConfigOpenApiEndpointViewModel.UseNetworkCredentials = serviceConfig.UseNetworkCredentials;
                ConfigOpenApiEndpointViewModel.WebProxyUri = serviceConfig.WebProxyUri;
                ConfigOpenApiEndpointViewModel.Description =
                    "An OpenAPI (Swagger) specification endpoint and generation options was regenerated";
                if (ConfigOpenApiEndpointViewModel.View is ConfigOpenApiEndpoint сonfigOpenApiEndpoint)
                {
                    //сonfigOpenApiEndpoint.Endpoint.IsEnabled = false;
                    //сonfigOpenApiEndpoint.ServiceName.IsEnabled = false;
                }

                CSharpClientSettingsViewModel.Command = serviceConfig.OpenApiToCSharpClientCommand;
                CSharpClientSettingsViewModel.Description = "Settings for generating CSharp client (regenerated)";
                TypeScriptClientSettingsViewModel.Command = serviceConfig.OpenApiToTypeScriptClientCommand;
                TypeScriptClientSettingsViewModel.Description = "Settings for generating TypeScript client (regenerated)";
                CSharpControllerSettingsViewModel.Command = serviceConfig.OpenApiToCSharpControllerCommand;
                CSharpControllerSettingsViewModel.Description = "Settings for generating CSharp controller (regenerated)";
            }

            this.Pages.Add(ConfigOpenApiEndpointViewModel);
            this.IsFinishEnabled = true;
        }

        public void AddCSharpClientSettingsPage()
        {
            if (!this.Pages.Contains(CSharpClientSettingsViewModel))
                this.Pages.Add(CSharpClientSettingsViewModel);
        }

        public void RemoveCSharpClientSettingsPage()
        {
            if (this.Pages.Contains(CSharpClientSettingsViewModel))
                this.Pages.Remove(CSharpClientSettingsViewModel);
        }

        public void AddTypeScriptClientSettingsPage()
        {
            if (!this.Pages.Contains(TypeScriptClientSettingsViewModel))
                this.Pages.Add(TypeScriptClientSettingsViewModel);
        }

        public void RemoveTypeScriptClientSettingsPage()
        {
            if (this.Pages.Contains(TypeScriptClientSettingsViewModel))
                this.Pages.Remove(TypeScriptClientSettingsViewModel);
        }

        public void AddCSharpControllerSettingsPage()
        {
            if (!this.Pages.Contains(CSharpControllerSettingsViewModel))
                this.Pages.Add(CSharpControllerSettingsViewModel);
        }

        public void RemoveCSharpControllerSettingsPage()
        {
            if (this.Pages.Contains(CSharpControllerSettingsViewModel))
                this.Pages.Remove(CSharpControllerSettingsViewModel);
        }

        public override Task<ConnectedServiceInstance> GetFinishedServiceInstanceAsync()
        {
            this.UserSettings.Save();

            this.ServiceInstance.Name = ConfigOpenApiEndpointViewModel.UserSettings.ServiceName;
            this.ServiceInstance.SpecificationTempPath = ConfigOpenApiEndpointViewModel.SpecificationTempPath;
            this.ServiceInstance.ServiceConfig = this.CreateServiceConfiguration();

            #region Network Credentials
            this.ServiceInstance.ServiceConfig.UseNetworkCredentials =
                ConfigOpenApiEndpointViewModel.UseNetworkCredentials;
            this.ServiceInstance.ServiceConfig.NetworkCredentialsUserName =
                ConfigOpenApiEndpointViewModel.NetworkCredentialsUserName;
            this.ServiceInstance.ServiceConfig.NetworkCredentialsPassword =
                ConfigOpenApiEndpointViewModel.NetworkCredentialsPassword;
            this.ServiceInstance.ServiceConfig.NetworkCredentialsDomain =
                ConfigOpenApiEndpointViewModel.NetworkCredentialsDomain;
            #endregion

            #region Web-proxy
            this.ServiceInstance.ServiceConfig.UseWebProxy =
                ConfigOpenApiEndpointViewModel.UseWebProxy;
            this.ServiceInstance.ServiceConfig.UseWebProxyCredentials =
                ConfigOpenApiEndpointViewModel.UseWebProxyCredentials;
            this.ServiceInstance.ServiceConfig.WebProxyNetworkCredentialsUserName =
                ConfigOpenApiEndpointViewModel.WebProxyNetworkCredentialsUserName;
            this.ServiceInstance.ServiceConfig.WebProxyNetworkCredentialsPassword =
                ConfigOpenApiEndpointViewModel.WebProxyNetworkCredentialsPassword;
            this.ServiceInstance.ServiceConfig.WebProxyNetworkCredentialsDomain =
                ConfigOpenApiEndpointViewModel.WebProxyNetworkCredentialsDomain;
            this.ServiceInstance.ServiceConfig.WebProxyUri =
                ConfigOpenApiEndpointViewModel.WebProxyUri;
            #endregion

            return Task.FromResult<ConnectedServiceInstance>(this.ServiceInstance);
        }

        /// <summary>
        /// Create the service configuration.
        /// </summary>
        private ServiceConfiguration CreateServiceConfiguration()
        {
            var serviceConfiguration = new ServiceConfiguration
            {
                ServiceName = string.IsNullOrWhiteSpace(ConfigOpenApiEndpointViewModel.UserSettings.ServiceName) ? Constants.DefaultServiceName : ConfigOpenApiEndpointViewModel.UserSettings.ServiceName,
                Endpoint = ConfigOpenApiEndpointViewModel.UserSettings.Endpoint,
                GeneratedFileNamePrefix = CSharpClientSettingsViewModel.GeneratedFileName,
                GenerateCSharpClient = ConfigOpenApiEndpointViewModel.UserSettings.GenerateCSharpClient,
                GenerateCSharpController = ConfigOpenApiEndpointViewModel.UserSettings.GenerateCSharpController,
                GenerateTypeScriptClient = ConfigOpenApiEndpointViewModel.UserSettings.GenerateTypeScriptClient,
                Variables = ConfigOpenApiEndpointViewModel.UserSettings.Variables,
                Runtime = ConfigOpenApiEndpointViewModel.UserSettings.Runtime,
                CopySpecification = ConfigOpenApiEndpointViewModel.UserSettings.CopySpecification,
                OpenGeneratedFilesOnComplete = ConfigOpenApiEndpointViewModel.UserSettings.OpenGeneratedFilesOnComplete
            };
            if (serviceConfiguration.GenerateCSharpClient && CSharpClientSettingsViewModel.Command != null)
                serviceConfiguration.OpenApiToCSharpClientCommand = CSharpClientSettingsViewModel.Command;

            if (serviceConfiguration.GenerateTypeScriptClient && TypeScriptClientSettingsViewModel.Command != null)
                serviceConfiguration.OpenApiToTypeScriptClientCommand = TypeScriptClientSettingsViewModel.Command;

            if (serviceConfiguration.GenerateCSharpController && CSharpControllerSettingsViewModel.Command != null)
                serviceConfiguration.OpenApiToCSharpControllerCommand = CSharpControllerSettingsViewModel.Command;

            return serviceConfiguration;
        }

        /// <summary>
        /// Cleanup object references.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (this.CSharpClientSettingsViewModel != null)
                    {
                        this.CSharpClientSettingsViewModel.Dispose();
                        this.CSharpClientSettingsViewModel = null;
                    }

                    if (this.TypeScriptClientSettingsViewModel != null)
                    {
                        this.TypeScriptClientSettingsViewModel.Dispose();
                        this.TypeScriptClientSettingsViewModel = null;
                    }

                    if (this.CSharpControllerSettingsViewModel != null)
                    {
                        this.CSharpControllerSettingsViewModel.Dispose();
                        this.CSharpControllerSettingsViewModel = null;
                    }

                    if (this.ConfigOpenApiEndpointViewModel != null)
                    {
                        this.ConfigOpenApiEndpointViewModel.Dispose();
                        this.ConfigOpenApiEndpointViewModel = null;
                    }

                    if (this._serviceInstance != null)
                    {
                        this._serviceInstance.Dispose();
                        this._serviceInstance = null;
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}
