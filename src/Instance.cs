using Microsoft.VisualStudio.ConnectedServices;
using Unchase.OpenAPI.ConnectedService.Models;

namespace Unchase.OpenAPI.ConnectedService
{
    internal class Instance : ConnectedServiceInstance
    {
        #region Properties
        public ServiceConfiguration ServiceConfig { get; set; }

        public string SpecificationTempPath { get; set; }
        #endregion

        #region Constructors
        public Instance()
        {
            InstanceId = Constants.ExtensionCategory;
            Name = Constants.DefaultServiceName;
        }
        #endregion
    }
}
