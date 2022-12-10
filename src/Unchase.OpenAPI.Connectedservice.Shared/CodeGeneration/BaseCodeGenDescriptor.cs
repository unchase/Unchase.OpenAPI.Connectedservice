//-----------------------------------------------------------------------
// <copyright file="BaseCodeGenDescriptor.cs" company="Unchase">
//     Copyright (c) Nikolay Chebotov (Unchase). All rights reserved.
// </copyright>
// <license>https://github.com/unchase/Unchase.OpenAPI.Connectedservice/blob/master/LICENSE.md</license>
// <author>Nickolay Chebotov (Unchase), spiritkola@hotmail.com</author>
//-----------------------------------------------------------------------

using System;
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

        public ConnectedServiceHandlerContext Context { get; }

        public Project Project { get; }

        public string ServiceUri { get; }

        public Instance Instance { get; }

        #endregion

        #region Constructors

        protected BaseCodeGenDescriptor(ConnectedServiceHandlerContext context, Instance serviceInstance)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            InitNuGetInstaller();

            Instance = serviceInstance;

            if (serviceInstance.ServiceConfig.UseRelativePath)
            {
                var projectPath = context.ProjectHierarchy?.GetProject().Properties.Item("FullPath").Value.ToString();
                if (projectPath == null || !File.Exists(Path.Combine(projectPath, serviceInstance.ServiceConfig.Endpoint)))
                {
                    throw new ArgumentException("Please input the service endpoint with exists file path.", "Service Endpoint");
                }

                ServiceUri = Path.Combine(projectPath, serviceInstance.ServiceConfig.Endpoint);
            }
            else
            {
                ServiceUri = serviceInstance.ServiceConfig.Endpoint;
            }

            Context = context;
            Project = context.ProjectHierarchy.GetProject();
        }
        private void InitNuGetInstaller()
        {
            var componentModel = (IComponentModel)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SComponentModel));
            PackageInstallerServices = componentModel.GetService<IVsPackageInstallerServices>();
            PackageInstaller = componentModel.GetService<IVsPackageInstaller>();
        }

        #endregion

        #region Methods

        public abstract Task AddNugetPackagesAsync();

        public abstract Task<string> AddGeneratedCodeAsync();

        public abstract Task<string> AddGeneratedNSwagFileAsync();

        protected string GetReferenceFileFolder()
        {
            var serviceReferenceFolderName = Context.HandlerHelper.GetServiceArtifactsRootFolder();
            return Path.Combine(Project.GetServiceFolderPath(serviceReferenceFolderName, Context.ServiceInstance.Name));
        }

        #endregion
    }
}