using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
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
    internal class ConfigOpenApiEndpointViewModel :
        ConnectedServiceWizardPage
    {
        #region Properties and Fields

        public string Endpoint { get; set; }

        public string CurrentVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public string NSwagVersion => Assembly.GetAssembly(typeof(NSwagCommandProcessor)).GetName().Version.ToString();

        public string EndpointDescription => $"{Endpoint} (GenerateCSharpClient: {UserSettings.GenerateCSharpClient}, GenerateCSharpController: {UserSettings.GenerateCSharpController}, GenerateTypeScriptClient: {UserSettings.GenerateTypeScriptClient}).";

        public string ServiceName { get; set; }

        private bool _acceptAllUntrustedCertificates;
        public bool AcceptAllUntrustedCertificates
        {
            get => _acceptAllUntrustedCertificates;
            set
            {
                _acceptAllUntrustedCertificates = value;
                UserSettings.AcceptAllUntrustedCertificates = value;
                OnPropertyChanged();
            }
        }

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

        /// <summary>
        /// Gets the available runtimes.
        /// </summary>
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

        public ConfigOpenApiEndpointViewModel(UserSettings userSettings, Wizard wizard)
        {
            Title = "Configure specification endpoint";
            Description = "Enter or choose an specification endpoint and check generation options to begin";
            Legend = "Specification Endpoint";
            InternalWizard = wizard;
            View = new ConfigOpenApiEndpoint(InternalWizard) { DataContext = this };
            UserSettings = userSettings;
            ServiceName = string.IsNullOrWhiteSpace(userSettings.ServiceName) ? Constants.DefaultServiceName : userSettings.ServiceName;
            AcceptAllUntrustedCertificates = userSettings.AcceptAllUntrustedCertificates;
            GeneratedFileName = string.IsNullOrWhiteSpace(userSettings.GeneratedFileName) ? Constants.DefaultGeneratedFileName : userSettings.GeneratedFileName;
            Endpoint = userSettings.Endpoint;
            UseNetworkCredentials = false;
            UseWebProxy = false;
            UseWebProxyCredentials = false;
        }

        #endregion

        #region Methods

        public override async Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            UserSettings.AddToTopOfMruList(((Wizard)Wizard).UserSettings.MruEndpoints, Endpoint);
            try
            {
                if (!UserSettings.GenerateCSharpClient && !UserSettings.GenerateCSharpController &&
                    !UserSettings.GenerateTypeScriptClient)
                {
                    throw new Exception("Should check one of the checkboxes for generating!");
                }

                SpecificationTempPath = await GetSpecificationAsync();

                // convert from OData specification
                if (UserSettings.ConvertFromOdata)
                {
                    var csdl = File.ReadAllText(SpecificationTempPath);
                    var model = CsdlReader.Parse(XElement.Parse(csdl).CreateReader());
                    if ((UserSettings.OpenApiConvertSettings.ServiceRoot == null || UserSettings.OpenApiConvertSettings.ServiceRoot.Host.Contains("localhost"))
                        && UserSettings.Endpoint.StartsWith("http", StringComparison.Ordinal))
                    {
                        UserSettings.OpenApiConvertSettings.ServiceRoot = new Uri(UserSettings.Endpoint.TrimEnd("$metadata".ToCharArray()));
                    }

                    var document = model.ConvertToOpenApi(UserSettings.OpenApiConvertSettings);
                    var outputJson = document.SerializeAsJson(UserSettings.OpenApiSpecVersion);
                    File.WriteAllText(SpecificationTempPath, outputJson);
                }

                return await base.OnPageLeavingAsync(args);
            }
            catch (Exception e)
            {
                return await Task.FromResult(
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
            if (string.IsNullOrEmpty(UserSettings.Endpoint))
            {
                throw new ArgumentNullException("Specification Endpoint", "Please input the specification endpoint.");
            }

            //if (!this.UserSettings.Endpoint.EndsWith(".json", StringComparison.Ordinal))
            //    throw new ArgumentException("Please input the OpenAPI (Swagger) specification endpoint ends with \".json\".", "OpenAPI (Swagger) Specification Endpoint");

            var workFile = Path.GetTempFileName();

            try
            {
                // from url
                if (UserSettings.Endpoint.StartsWith("http://", StringComparison.Ordinal) || UserSettings.Endpoint.StartsWith("https://", StringComparison.Ordinal))
                {
                    // add $metadata to OData specification url
                    if (UserSettings.ConvertFromOdata &&
                        !UserSettings.Endpoint.EndsWith("$metadata", StringComparison.Ordinal))
                    {
                        UserSettings.Endpoint = UserSettings.Endpoint.TrimEnd('/') + "/$metadata";
                    }


                    var httpHandler = new HttpClientHandler();

                    if (UseWebProxy)
                    {
                        var proxy = new WebProxy(WebProxyUri, true); /*WebRequest.DefaultWebProxy*/

                        proxy.Credentials = UseWebProxyCredentials
                            ? new NetworkCredential(WebProxyNetworkCredentialsUserName,
                                WebProxyNetworkCredentialsPassword, WebProxyNetworkCredentialsDomain)
                            : CredentialCache.DefaultCredentials;
                        httpHandler.UseProxy = UseWebProxy;
                        httpHandler.Proxy = proxy;
                        httpHandler.PreAuthenticate = UseWebProxy;
                    }

                    if (UseNetworkCredentials)
                    {
                        httpHandler.UseDefaultCredentials = UseNetworkCredentials;
                        httpHandler.Credentials = new NetworkCredential(NetworkCredentialsUserName,
                            NetworkCredentialsPassword, NetworkCredentialsDomain);
                    }

                    if (UserSettings.AcceptAllUntrustedCertificates)
                    {
                        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                    }

                    using (var httpClient = new HttpClient(httpHandler))
                    {
                        var specificationEndpoint = await httpClient.GetStringAsync(UserSettings.Endpoint);
                        if (string.IsNullOrWhiteSpace(specificationEndpoint))
                        {
                            throw new InvalidOperationException("The specification endpoint is an empty.");
                        }

                        if (UserSettings.Endpoint.EndsWith(".json") && !ProjectHelper.IsJson(specificationEndpoint))
                        {
                            throw new InvalidOperationException("Service endpoint ends with \".json\" is not an json.");
                        }

                        File.WriteAllText(workFile, specificationEndpoint);
                    }
                }
                else // from local path
                {
                    var specificationEndpoint = File.ReadAllText(InternalWizard.GetEndpointPath(UserSettings.Endpoint, UserSettings.UseRelativePath));
                    if (string.IsNullOrWhiteSpace(specificationEndpoint))
                    {
                        throw new InvalidOperationException("The specification endpoint is an empty file.");
                    }

                    if (UserSettings.Endpoint.EndsWith(".json") && !ProjectHelper.IsJson(specificationEndpoint))
                    {
                        throw new InvalidOperationException("Service endpoint ends with \".json\" is not an json-file.");
                    }

                    File.WriteAllText(workFile, specificationEndpoint);
                }

                return workFile;
            }
            catch (WebException e)
            {
                throw new InvalidOperationException($"Cannot access \"{UserSettings.Endpoint}\". {e.Message}", e);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Cannot access \"{UserSettings.Endpoint}\". {e.Message}", e);
            }
        }

        #endregion
    }
}