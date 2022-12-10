//-----------------------------------------------------------------------
// <copyright file="Wizard.cs" company="Unchase">
//     Copyright (c) Nikolay Chebotov (Unchase). All rights reserved.
// </copyright>
// <license>https://github.com/unchase/Unchase.OpenAPI.Connectedservice/blob/master/LICENSE.md</license>
// <author>Nickolay Chebotov (Unchase), spiritkola@hotmail.com</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.OpenApi.OData;
using Microsoft.VisualStudio.ConnectedServices;
using Unchase.OpenAPI.ConnectedService.Common;
using Unchase.OpenAPI.ConnectedService.Models;
using Unchase.OpenAPI.ConnectedService.ViewModels;
using Unchase.OpenAPI.ConnectedService.Views;

namespace Unchase.OpenAPI.ConnectedService
{
    internal class Wizard :
        ConnectedServiceWizard
    {
        #region Properties and Fields

        private Instance _serviceInstance;

        internal readonly string ProjectPath;

        public ConfigOpenApiEndpointViewModel ConfigOpenApiEndpointViewModel { get; set; }

        public CSharpClientSettingsViewModel CSharpClientSettingsViewModel { get; set; }

        public TypeScriptClientSettingsViewModel TypeScriptClientSettingsViewModel { get; set; }

        public CSharpControllerSettingsViewModel CSharpControllerSettingsViewModel { get; set; }

        public ConnectedServiceProviderContext Context { get; set; }

        public Instance ServiceInstance => _serviceInstance ?? (_serviceInstance = new Instance());

        public UserSettings UserSettings { get; }

        #endregion

        #region Constructors

        public Wizard(ConnectedServiceProviderContext context)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Context = context;
            ProjectPath = context.ProjectHierarchy?.GetProject().Properties.Item("FullPath").Value.ToString();
            UserSettings = UserSettings.Load(context.Logger);
            UserSettings.ProjectPath = ProjectPath;

            if (Context.IsUpdating)
            {
                UserSettings.SetFromServiceConfiguration(context.GetExtendedDesignerData<ServiceConfiguration>());
            }

            ConfigOpenApiEndpointViewModel = new ConfigOpenApiEndpointViewModel(UserSettings, this);
            CSharpClientSettingsViewModel = new CSharpClientSettingsViewModel();
            TypeScriptClientSettingsViewModel = new TypeScriptClientSettingsViewModel();
            CSharpControllerSettingsViewModel = new CSharpControllerSettingsViewModel();

            if (Context.IsUpdating)
            {
                var serviceConfig = Context.GetExtendedDesignerData<ServiceConfiguration>();
                ConfigOpenApiEndpointViewModel.Endpoint = serviceConfig.Endpoint;
                ConfigOpenApiEndpointViewModel.ServiceName = serviceConfig.ServiceName;
                ConfigOpenApiEndpointViewModel.AcceptAllUntrustedCertificates = serviceConfig.AcceptAllUntrustedCertificates;
                ConfigOpenApiEndpointViewModel.GeneratedFileName = serviceConfig.GeneratedFileName;
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
                if (ConfigOpenApiEndpointViewModel.View is ConfigOpenApiEndpoint configOpenApiEndpoint)
                {
                    configOpenApiEndpoint.Endpoint.IsEnabled = false;
                    configOpenApiEndpoint.UseRelativePath.IsEnabled = false;
                    configOpenApiEndpoint.ServiceName.IsEnabled = false;
                    configOpenApiEndpoint.GeneratedFileName.IsEnabled = false;
                    configOpenApiEndpoint.ConvertFromOdata.IsEnabled = false;
                    configOpenApiEndpoint.OpenEndpointFileButton.IsEnabled = false;
                    configOpenApiEndpoint.GenerateCSharpClient.IsEnabled = false;
                    configOpenApiEndpoint.GenerateTypeScriptClient.IsEnabled = false;
                    configOpenApiEndpoint.GenerateCSharpController.IsEnabled = false;
                }

                CSharpClientSettingsViewModel.Command = serviceConfig.OpenApiToCSharpClientCommand;
                CSharpClientSettingsViewModel.ExcludeTypeNamesLater = serviceConfig.ExcludeTypeNamesLater;
                CSharpClientSettingsViewModel.Description = "Settings for generating CSharp client (regenerated)";
                if (CSharpClientSettingsViewModel.View is CSharpClientSettings cSharpClientSettings)
                {
                    cSharpClientSettings.Namespace.IsEnabled = false;
                    cSharpClientSettings.AdditionalNamespaceUsages.IsEnabled = false;
                }

                TypeScriptClientSettingsViewModel.Command = serviceConfig.OpenApiToTypeScriptClientCommand;
                TypeScriptClientSettingsViewModel.Description = "Settings for generating TypeScript client (regenerated)";
                if (TypeScriptClientSettingsViewModel.View is TypeScriptClientSettings typeScriptClientSettings)
                {
                    typeScriptClientSettings.Namespace.IsEnabled = false;
                }

                CSharpControllerSettingsViewModel.Command = serviceConfig.OpenApiToCSharpControllerCommand;
                CSharpControllerSettingsViewModel.Description = "Settings for generating CSharp controller (regenerated)";
                if (CSharpControllerSettingsViewModel.View is CSharpControllerSettings cSharpControllerSettings)
                {
                    cSharpControllerSettings.Namespace.IsEnabled = false;
                    cSharpControllerSettings.AdditionalNamespaceUsages.IsEnabled = false;
                }
            }

            Pages.Add(ConfigOpenApiEndpointViewModel);
            IsFinishEnabled = true;
        }

        #endregion

        #region Methods

        public void AddCSharpClientSettingsPage()
        {
            if (!Pages.Contains(CSharpClientSettingsViewModel))
            {
                Pages.Add(CSharpClientSettingsViewModel);
            }
        }

        public void RemoveCSharpClientSettingsPage()
        {
            if (Pages.Contains(CSharpClientSettingsViewModel))
            {
                Pages.Remove(CSharpClientSettingsViewModel);
            }
        }

        public void AddTypeScriptClientSettingsPage()
        {
            if (!Pages.Contains(TypeScriptClientSettingsViewModel))
            {
                Pages.Add(TypeScriptClientSettingsViewModel);
            }
        }

        public void RemoveTypeScriptClientSettingsPage()
        {
            if (Pages.Contains(TypeScriptClientSettingsViewModel))
            {
                Pages.Remove(TypeScriptClientSettingsViewModel);
            }
        }

        public void AddCSharpControllerSettingsPage()
        {
            if (!Pages.Contains(CSharpControllerSettingsViewModel))
            {
                Pages.Add(CSharpControllerSettingsViewModel);
            }
        }

        public void RemoveCSharpControllerSettingsPage()
        {
            if (Pages.Contains(CSharpControllerSettingsViewModel))
            {
                Pages.Remove(CSharpControllerSettingsViewModel);
            }
        }

        public override Task<ConnectedServiceInstance> GetFinishedServiceInstanceAsync()
        {
            UserSettings.Save();

            ServiceInstance.Name = ConfigOpenApiEndpointViewModel.UserSettings.ServiceName;
            ServiceInstance.SpecificationTempPath = ConfigOpenApiEndpointViewModel.SpecificationTempPath;
            ServiceInstance.ServiceConfig = CreateServiceConfiguration();

            #region Network Credentials

            ServiceInstance.ServiceConfig.UseNetworkCredentials =
                ConfigOpenApiEndpointViewModel.UseNetworkCredentials;
            ServiceInstance.ServiceConfig.NetworkCredentialsUserName =
                ConfigOpenApiEndpointViewModel.NetworkCredentialsUserName;
            ServiceInstance.ServiceConfig.NetworkCredentialsPassword =
                ConfigOpenApiEndpointViewModel.NetworkCredentialsPassword;
            ServiceInstance.ServiceConfig.NetworkCredentialsDomain =
                ConfigOpenApiEndpointViewModel.NetworkCredentialsDomain;

            #endregion

            #region Web-proxy

            ServiceInstance.ServiceConfig.UseWebProxy =
                ConfigOpenApiEndpointViewModel.UseWebProxy;
            ServiceInstance.ServiceConfig.UseWebProxyCredentials =
                ConfigOpenApiEndpointViewModel.UseWebProxyCredentials;
            ServiceInstance.ServiceConfig.WebProxyNetworkCredentialsUserName =
                ConfigOpenApiEndpointViewModel.WebProxyNetworkCredentialsUserName;
            ServiceInstance.ServiceConfig.WebProxyNetworkCredentialsPassword =
                ConfigOpenApiEndpointViewModel.WebProxyNetworkCredentialsPassword;
            ServiceInstance.ServiceConfig.WebProxyNetworkCredentialsDomain =
                ConfigOpenApiEndpointViewModel.WebProxyNetworkCredentialsDomain;
            ServiceInstance.ServiceConfig.WebProxyUri =
                ConfigOpenApiEndpointViewModel.WebProxyUri;

            #endregion

            return Task.FromResult<ConnectedServiceInstance>(ServiceInstance);
        }

        /// <summary>
        /// Create the service configuration.
        /// </summary>
        private ServiceConfiguration CreateServiceConfiguration()
        {
            var serviceConfiguration = new ServiceConfiguration
            {
                ServiceName = string.IsNullOrWhiteSpace(ConfigOpenApiEndpointViewModel.UserSettings.ServiceName) ? Constants.DefaultServiceName : ConfigOpenApiEndpointViewModel.UserSettings.ServiceName,
                AcceptAllUntrustedCertificates = ConfigOpenApiEndpointViewModel.UserSettings.AcceptAllUntrustedCertificates,
                GeneratedFileName = string.IsNullOrWhiteSpace(ConfigOpenApiEndpointViewModel.UserSettings.GeneratedFileName) ? Constants.DefaultGeneratedFileName : ConfigOpenApiEndpointViewModel.UserSettings.GeneratedFileName,
                Endpoint = ConfigOpenApiEndpointViewModel.UserSettings.Endpoint,
                GeneratedFileNamePrefix = CSharpClientSettingsViewModel.GeneratedFileName,
                GenerateCSharpClient = ConfigOpenApiEndpointViewModel.UserSettings.GenerateCSharpClient,
                GenerateCSharpController = ConfigOpenApiEndpointViewModel.UserSettings.GenerateCSharpController,
                GenerateTypeScriptClient = ConfigOpenApiEndpointViewModel.UserSettings.GenerateTypeScriptClient,
                Variables = ConfigOpenApiEndpointViewModel.UserSettings.Variables,
                Runtime = ConfigOpenApiEndpointViewModel.UserSettings.Runtime,
                CopySpecification = ConfigOpenApiEndpointViewModel.UserSettings.CopySpecification,
                OpenGeneratedFilesOnComplete = ConfigOpenApiEndpointViewModel.UserSettings.OpenGeneratedFilesOnComplete,
                UseRelativePath = ConfigOpenApiEndpointViewModel.UserSettings.UseRelativePath,
                ConvertFromOdata = ConfigOpenApiEndpointViewModel.UserSettings.ConvertFromOdata,
                OpenApiConvertSettings = ConfigOpenApiEndpointViewModel.UserSettings.ConvertFromOdata
                    ? ConfigOpenApiEndpointViewModel.UserSettings.OpenApiConvertSettings : new OpenApiConvertSettings(),
                OpenApiSpecVersion = ConfigOpenApiEndpointViewModel.UserSettings.OpenApiSpecVersion
            };

            if (serviceConfiguration.GenerateCSharpClient && CSharpClientSettingsViewModel.Command != null)
            {
                serviceConfiguration.OpenApiToCSharpClientCommand = CSharpClientSettingsViewModel.Command;
                serviceConfiguration.ExcludeTypeNamesLater = CSharpClientSettingsViewModel.ExcludeTypeNamesLater;
            }

            if (serviceConfiguration.GenerateTypeScriptClient && TypeScriptClientSettingsViewModel.Command != null)
            {
                serviceConfiguration.OpenApiToTypeScriptClientCommand = TypeScriptClientSettingsViewModel.Command;
            }

            if (serviceConfiguration.GenerateCSharpController && CSharpControllerSettingsViewModel.Command != null)
            {
                serviceConfiguration.OpenApiToCSharpControllerCommand = CSharpControllerSettingsViewModel.Command;
            }

            if (string.IsNullOrWhiteSpace(serviceConfiguration.OpenApiConvertSettings?.PathPrefix))
            {
                serviceConfiguration.OpenApiConvertSettings.PathPrefix = Constants.OpenApiConvertSettingsPathPrefix;
            }

            return serviceConfiguration;
        }

        /// <summary>
        /// Get specification endpoint path.
        /// </summary>
        /// <param name="endpoint">Endpoint path (relative or absolute).</param>
        /// <param name="useRelativePath">Use relative path instead of absolute.</param>
        internal string GetEndpointPath(string endpoint, bool useRelativePath = false)
        {
            if (endpoint.StartsWith("http://", StringComparison.Ordinal)
                || endpoint.StartsWith("https://", StringComparison.Ordinal)
                || File.Exists(endpoint)
                || !useRelativePath)
            {
                return endpoint;
            }

            if (!File.Exists(Path.Combine(ProjectPath, endpoint)))
            {
                throw new ArgumentException("Please input the service endpoint with exists file path.", "Service Endpoint");
            }

            return Path.Combine(ProjectPath, endpoint);
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
                    if (CSharpClientSettingsViewModel != null)
                    {
                        CSharpClientSettingsViewModel.Dispose();
                        CSharpClientSettingsViewModel = null;
                    }

                    if (TypeScriptClientSettingsViewModel != null)
                    {
                        TypeScriptClientSettingsViewModel.Dispose();
                        TypeScriptClientSettingsViewModel = null;
                    }

                    if (CSharpControllerSettingsViewModel != null)
                    {
                        CSharpControllerSettingsViewModel.Dispose();
                        CSharpControllerSettingsViewModel = null;
                    }

                    if (ConfigOpenApiEndpointViewModel != null)
                    {
                        ConfigOpenApiEndpointViewModel.Dispose();
                        ConfigOpenApiEndpointViewModel = null;
                    }

                    if (_serviceInstance != null)
                    {
                        _serviceInstance.Dispose();
                        _serviceInstance = null;
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #endregion
    }
}