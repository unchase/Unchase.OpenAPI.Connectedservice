using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ConnectedServices;
using Newtonsoft.Json.Linq;
using NSwag.Commands;
using Unchase.OpenAPI.ConnectedService.Models;
using Unchase.OpenAPI.ConnectedService.Views;

namespace Unchase.OpenAPI.ConnectedService.ViewModels
{
    internal class ConfigOpenApiEndpointViewModel : ConnectedServiceWizardPage
    {
        public string Endpoint { get; set; }

        public string EndpointDescription => $"{this.Endpoint} (GenerateCSharpClient: {UserSettings.GenerateCSharpClient}, GenerateCSharpController: {UserSettings.GenerateCSharpController}, GenerateTypeScriptClient: {UserSettings.GenerateTypeScriptClient}).";

        public string ServiceName { get; set; }

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

        public ConfigOpenApiEndpointViewModel(UserSettings userSettings, Wizard wizard) : base()
        {
            this.Title = "Configure specification endpoint";
            this.Description = "Enter or choose an OpenAPI (Swagger) specification endpoint and check generation options to begin";
            this.Legend = "Specification Endpoint";
            this.InternalWizard = wizard;
            this.View = new ConfigOpenApiEndpoint(this.InternalWizard) {DataContext = this};
            this.UserSettings = userSettings;
            this.ServiceName = userSettings.ServiceName ?? Constants.DefaultServiceName;
            this.Endpoint = userSettings.Endpoint;
            this.UseNetworkCredentials = false;
            this.UseWebProxy = false;
            this.UseWebProxyCredentials = false;
        }

        public override async Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            UserSettings.AddToTopOfMruList(((Wizard)this.Wizard).UserSettings.MruEndpoints, this.Endpoint);
            try
            {
                if (!UserSettings.GenerateCSharpClient && !UserSettings.GenerateCSharpController && !UserSettings.GenerateTypeScriptClient)
                    throw new Exception("Should check one of the checkboxes for generating!");

                this.SpecificationTempPath = await GetSpecification();
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

        internal async Task<string> GetSpecification()
        {
            if (string.IsNullOrEmpty(this.UserSettings.Endpoint))
                throw new ArgumentNullException("OpenAPI (Swagger) Specification Endpoint", "Please input the OpenAPI (Swagger) specification endpoint.");

            if (!this.UserSettings.Endpoint.EndsWith(".json", StringComparison.Ordinal))
                throw new ArgumentException("Please input the OpenAPI (Swagger) specification endpoint ends with \".json\".", "OpenAPI (Swagger) Specification Endpoint");

            var workFile = Path.GetTempFileName();

            try
            {
                // from url
                if (this.UserSettings.Endpoint.StartsWith("http://", StringComparison.Ordinal) || this.UserSettings.Endpoint.StartsWith("https://", StringComparison.Ordinal))
                {
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
                        var specificationJson = await httpClient.GetStringAsync(this.UserSettings.Endpoint);
                        if (string.IsNullOrWhiteSpace(specificationJson))
                            throw new InvalidOperationException("The json-file is an empty file.");

                        if (!IsJson(specificationJson))
                            throw new InvalidOperationException("Service endpoint is not an json-file.");

                        File.WriteAllText(workFile, specificationJson);
                    }
                }
                else // from local path
                {
                    if (!File.Exists(this.UserSettings.Endpoint))
                        throw new ArgumentException("Please input the service endpoint with exists file path.", "OpenAPI Service Endpoint");

                    var specificationJson = File.ReadAllText(this.UserSettings.Endpoint);
                    if (!IsJson(specificationJson))
                        throw new InvalidOperationException("Service endpoint is not an json-file.");

                    File.WriteAllText(workFile, specificationJson);
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

        internal static bool IsJson(string input)
        {
            input = input.Trim();
            try
            {
                JToken.Parse(input);
            }
            catch
            {
                return false;
            }
            return input.StartsWith("{") && input.EndsWith("}")
                    || input.StartsWith("[") && input.EndsWith("]");
        }
    }
}
