//-----------------------------------------------------------------------
// <copyright file="BaseCodeGenDescriptor.cs" company="Unchase">
//     Copyright (c) Nikolay Chebotov (Unchase). All rights reserved.
// </copyright>
// <license>https://github.com/unchase/Unchase.OpenAPI.Connectedservice/blob/master/LICENSE.md</license>
// <author>Nickolay Chebotov (Unchase), spiritkola@hotmail.com</author>
//-----------------------------------------------------------------------

using System.IO;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.ConnectedServices;
using NuGet.VisualStudio;
using Unchase.OpenAPI.ConnectedService.Common;

namespace Unchase.OpenAPI.ConnectedService.CodeGeneration
{
    internal abstract class BaseCodeGenDescriptor
    {
        #region Properties
        public IVsPackageInstaller PackageInstaller { get; private set; }

        public IVsPackageInstallerServices PackageInstallerServices { get; private set; }

        public ConnectedServiceHandlerContext Context { get; private set; }

        public Project Project { get; private set; }

        public string ServiceUri { get; private set; }

        public Instance Instance { get; private set; }
        #endregion

        #region Constructors
        protected BaseCodeGenDescriptor(ConnectedServiceHandlerContext context, Instance serviceInstance)
        {
            this.InitNuGetInstaller();

            this.Instance = serviceInstance;
            this.ServiceUri = serviceInstance.ServiceConfig.Endpoint;
            this.Context = context;
            this.Project = context.ProjectHierarchy.GetProject();
        }
        private void InitNuGetInstaller()
        {
            var componentModel = (IComponentModel)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SComponentModel));
            this.PackageInstallerServices = componentModel.GetService<IVsPackageInstallerServices>();
            this.PackageInstaller = componentModel.GetService<IVsPackageInstaller>();
        }
        #endregion

        #region Methods
        public abstract Task AddNugetPackagesAsync();

        public abstract Task<string> AddGeneratedCodeAsync();

        public abstract Task<string> AddGeneratedNswagFileAsync();

        protected string GetReferenceFileFolder()
        {
            var serviceReferenceFolderName = this.Context.HandlerHelper.GetServiceArtifactsRootFolder();
            return Path.Combine(ProjectHelper.GetServiceFolderPath(this.Project, serviceReferenceFolderName, this.Context.ServiceInstance.Name));
        }
        #endregion
    }
}
