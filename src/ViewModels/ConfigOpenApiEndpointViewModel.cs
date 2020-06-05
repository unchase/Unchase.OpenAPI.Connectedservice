using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.OData;
using Microsoft.VisualStudio.ConnectedServices;
using NSwag.Commands;
using Unchase.OpenAPI.ConnectedService.Common;
using Unchase.OpenAPI.ConnectedService.Models;
using Unchase.OpenAPI.ConnectedService.Views;

namespace Unchase.OpenAPI.ConnectedService.ViewModels
{
    internal class ConfigOpenApiEndpointViewModel : ConnectedServiceWizardPage
    {
        #region Properties and Fields
        public string Endpoint { get; set; }

        public string EndpointDescription => $"{this.Endpoint} (GenerateCSharpClient: {UserSettings.GenerateCSharpClient}, GenerateCSharpController: {UserSettings.GenerateCSharpController}, GenerateTypeScriptClient: {UserSettings.GenerateTypeScriptClient}).";

        public string ServiceName { get; set; }

        public string GeneratedFileName { get; set; }

        public string SpecificationTempPath { get; set; }

        public UserSettings UserSettings { get; }

        public Wizard InternalWizard;

        #region Network Credentials
        public bool UseNetworkCredentials { get; set; }
        public string NetworkCredentialsUserName { get; set; }
        public string NetworkCredentialsPassword { get; set; }
        public string NetworkCredentialsDomain { get; set; }
        #endregion

        #region WebProxy
        public bool UseWebProxy { get; set; }
        public bool UseWebProxyCredentials { get; set; }
        public string WebProxyNetworkCredentialsUserName { get; set; }
        public string WebProxyNetworkCredentialsPassword { get; set; }
        public string WebProxyNetworkCredentialsDomain { get; set; }
        public string WebProxyUri { get; set; }
        #endregion

        /// <summary>Gets the available runtimes.</summary>
        public Runtime[] Runtimes
        {
            get
            {
                return Enum.GetNames(typeof(Runtime))
                    .Select(t => (Runtime)Enum.Parse(typeof(Runtime), t))
                    .ToArray();
            }
        }

        public OpenApiSpecVersion[] OpenApiSpecVersions
        {
            get
            {
                return Enum.GetNames(typeof(OpenApiSpecVersion))
                    .Select(t => (OpenApiSpecVersion)Enum.Parse(typeof(OpenApiSpecVersion), t))
                    .ToArray();
            }
        }

        #endregion

        #region Constructors
        public ConfigOpenApiEndpointViewModel(UserSettings userSettings, Wizard wizard) : base()
        {
            this.Title = "Configure specification endpoint";
            this.Description = "Enter or choose an specification endpoint and check generation options to begin";
            this.Legend = "Specification Endpoint";
            this.InternalWizard = wizard;
            this.View = new ConfigOpenApiEndpoint(this.InternalWizard) {DataContext = this};
            this.UserSettings = userSettings;
            this.ServiceName = string.IsNullOrWhiteSpace(userSettings.ServiceName) ? Constants.DefaultServiceName : userSettings.ServiceName;
            this.GeneratedFileName = string.IsNullOrWhiteSpace(userSettings.GeneratedFileName) ? Constants.DefaultGeneratedFileName : userSettings.GeneratedFileName;
            this.Endpoint = userSettings.Endpoint;
            this.UseNetworkCredentials = false;
            this.UseWebProxy = false;
            this.UseWebProxyCredentials = false;
        }
        #endregion

        #region Methods
        public override async Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            UserSettings.AddToTopOfMruList(((Wizard)this.Wizard).UserSettings.MruEndpoints, this.Endpoint);
            try
            {
                if (!UserSettings.GenerateCSharpClient && !UserSettings.GenerateCSharpController && !UserSettings.GenerateTypeScriptClient)
                    throw new Exception("Should check one of the checkboxes for generating!");

                this.SpecificationTempPath = await GetSpecificationAsync();

                // convert from OData specification
                if (this.UserSettings.ConvertFromOdata)
                {
                    var csdl = File.ReadAllText(this.SpecificationTempPath);
                    var model = CsdlReader.Parse(XElement.Parse(csdl).CreateReader());
                    if (this.UserSettings.OpenApiConvertSettings.ServiceRoot == null && this.UserSettings.Endpoint.StartsWith("http", StringComparison.Ordinal))
                        this.UserSettings.OpenApiConvertSettings.ServiceRoot = new Uri(this.UserSettings.Endpoint.TrimEnd("$metadata".ToCharArray()));
                    var document = model.ConvertToOpenApi(this.UserSettings.OpenApiConvertSettings);
                    var outputJson = document.SerializeAsJson(this.UserSettings.OpenApiSpecVersion);
                    File.WriteAllText(this.SpecificationTempPath, outputJson);
                }

                return await base.OnPageLeavingAsync(args);
            }
            catch (Exception e)
            {
                return await Task.FromResult<PageNavigationResult>(
                    new PageNavigationResult
                    {
                        ErrorMessage = e.Message,
                        IsSuccess = false,
                        ShowMessageBoxOnFailure = true
                    });
            }
        }

        internal async Task<string> GetSpecificationAsync()
        {
            if (string.IsNullOrEmpty(this.UserSettings.Endpoint))
                throw new ArgumentNullException("Specification Endpoint", "Please input the specification endpoint.");

            //if (!this.UserSettings.Endpoint.EndsWith(".json", StringComparison.Ordinal))
            //    throw new ArgumentException("Please input the OpenAPI (Swagger) specification endpoint ends with \".json\".", "OpenAPI (Swagger) Specification Endpoint");

            var workFile = Path.GetTempFileName();

            try
            {
                // from url
                if (this.UserSettings.Endpoint.StartsWith("http://", StringComparison.Ordinal) || this.UserSettings.Endpoint.StartsWith("https://", StringComparison.Ordinal))
                {
                    // add $metadata to OData specification url
                    if (this.UserSettings.ConvertFromOdata && !this.UserSettings.Endpoint.EndsWith("$metadata", StringComparison.Ordinal))
                        this.UserSettings.Endpoint = this.UserSettings.Endpoint.TrimEnd('/') + "/$metadata";

                    var proxy = this.UseWebProxy ? new WebProxy(this.WebProxyUri, true) : WebProxy.GetDefaultProxy();
                    proxy.Credentials = this.UseWebProxy && this.UseWebProxyCredentials
                        ? new NetworkCredential(this.WebProxyNetworkCredentialsUserName,
                            this.WebProxyNetworkCredentialsPassword, this.WebProxyNetworkCredentialsDomain)
                        : CredentialCache.DefaultCredentials;
                    var httpHandler = new HttpClientHandler();

                    if (this.UseWebProxy)
                    {
                        httpHandler.UseProxy = this.UseWebProxy;
                        httpHandler.Proxy = proxy;
                        httpHandler.PreAuthenticate = this.UseWebProxy;
                    }

                    if (this.UseNetworkCredentials)
                    {
                        httpHandler.UseDefaultCredentials = this.UseNetworkCredentials;
                        httpHandler.Credentials = new NetworkCredential(this.NetworkCredentialsUserName,
                            this.NetworkCredentialsPassword, this.NetworkCredentialsDomain);
                    }

                    using (var httpClient = new HttpClient(httpHandler))
                    {
                        var specificationEndpoint = await httpClient.GetStringAsync(this.UserSettings.Endpoint);
                        if (string.IsNullOrWhiteSpace(specificationEndpoint))
                            throw new InvalidOperationException("The specification endpoint is an empty.");

                        if (this.UserSettings.Endpoint.EndsWith(".json") && !ProjectHelper.IsJson(specificationEndpoint))
                            throw new InvalidOperationException("Service endpoint ends with \".json\" is not an json.");

                        File.WriteAllText(workFile, specificationEndpoint);
                    }
                }
                else // from local path
                {
                    var specificationEndpoint = File.ReadAllText(this.InternalWizard.GetEndpointPath(this.UserSettings.Endpoint, this.UserSettings.UseRelativePath));
                    if (string.IsNullOrWhiteSpace(specificationEndpoint))
                        throw new InvalidOperationException("The specification endpoint is an empty file.");

                    if (this.UserSettings.Endpoint.EndsWith(".json") && !ProjectHelper.IsJson(specificationEndpoint))
                        throw new InvalidOperationException("Service endpoint ends with \".json\" is not an json-file.");

                    File.WriteAllText(workFile, specificationEndpoint);
                }

                return workFile;
            }
            catch (WebException e)
            {
                throw new InvalidOperationException($"Cannot access \"{this.UserSettings.Endpoint}\". {e.Message}", e);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Cannot access \"{this.UserSettings.Endpoint}\". {e.Message}", e);
            }
        }
        #endregion
    }
}
