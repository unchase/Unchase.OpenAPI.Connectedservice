using NSwag.Commands;
using NSwag.Commands.CodeGeneration;

namespace Unchase.OpenAPI.ConnectedService.Models
{
    internal class ServiceConfiguration
    {
        #region Properties
        public string ServiceName { get; set; }

        public string Endpoint { get; set; }

        public string GeneratedFileNamePrefix { get; set; }

        public bool GenerateCSharpClient { get; set; } = false;

        public bool GenerateTypeScriptClient { get; set; } = false;

        public bool GenerateCSharpController { get; set; } = false;

        public OpenApiToCSharpClientCommand OpenApiToCSharpClientCommand { get; set; }

        public OpenApiToTypeScriptClientCommand OpenApiToTypeScriptClientCommand { get; set; }

        public OpenApiToCSharpControllerCommand OpenApiToCSharpControllerCommand { get; set; }

        public string Variables { get; set; }

        public Runtime Runtime { get; set; }

        public bool CopySpecification { get; set; }

        public bool OpenGeneratedFilesOnComplete { get; set; }

        public bool UseRelativePath { get; set; }
        #endregion

        #region Network Credentials
        public bool UseNetworkCredentials { get; set; }
        public string NetworkCredentialsUserName { get; set; }
        public string NetworkCredentialsPassword { get; set; }
        public string NetworkCredentialsDomain { get; set; }
        #endregion

        #region WebProxy
        public string WebProxyUri { get; set; }
        public bool UseWebProxy { get; set; }
        public bool UseWebProxyCredentials { get; set; }
        public string WebProxyNetworkCredentialsUserName { get; set; }
        public string WebProxyNetworkCredentialsPassword { get; set; }
        public string WebProxyNetworkCredentialsDomain { get; set; }
        #endregion
    }
}
