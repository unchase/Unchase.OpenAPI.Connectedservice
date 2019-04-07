using Microsoft.VisualStudio.ConnectedServices;
using Unchase.OpenAPI.ConnectedService.Models;

namespace Unchase.OpenAPI.ConnectedService
{
    internal class Instance : ConnectedServiceInstance
    {
        public ServiceConfiguration ServiceConfig { get; set; }
        public string SpecificationTempPath { get; set; }

        public Instance()
        {
            InstanceId = Constants.ExtensionCategory;
            Name = Constants.DefaultServiceName;
        }
    }
}
