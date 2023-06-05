//-----------------------------------------------------------------------
// <copyright file="BaseCodeGenDescriptor.cs" company="Unchase">
//     Copyright (c) Nikolay Chebotov (Unchase). All rights reserved.
// </copyright>
// <license>https://github.com/unchase/Unchase.OpenAPI.Connectedservice/blob/master/LICENSE.md</license>
// <author>Nickolay Chebotov (Unchase), spiritkola@hotmail.com</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.ConnectedServices;
using Microsoft.VisualStudio.Shell;
using NuGet.VisualStudio;
using Unchase.OpenAPI.ConnectedService.Common;
using Task = System.Threading.Tasks.Task;

namespace Unchase.OpenAPI.ConnectedService.CodeGeneration
{
    internal abstract class BaseCodeGenDescriptor
    {
        #region Properties

        public IVsPackageInstaller PackageInstaller { get; private set; }

        public IVsPackageInstallerServices PackageInstallerServices { get; private set; }

        public ConnectedServiceHandlerContext Context { get; }

        public Project Project { get; private set; }

        public string ServiceUri { get; private set; }

        public Instance Instance { get; }

        #endregion

        #region Constructors

        protected BaseCodeGenDescriptor(ConnectedServiceHandlerContext context, Instance serviceInstance)
        {
            Instance = serviceInstance;
            Context = context;
        }

        protected virtual async Task InitializeAsync()
        {
            await InitNuGetInstallerAsync();

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            Project = Context.ProjectHierarchy?.GetProject();

            var serviceConfig = Instance.ServiceConfig;
            if (serviceConfig.UseRelativePath)
            {
                var projectPath = Project?.Properties.Item("FullPath").Value.ToString();
                if (projectPath == null || !File.Exists(Path.Combine(projectPath, serviceConfig.Endpoint)))
                {
                    throw new ArgumentException("Please input the service endpoint with exists file path.", "Service Endpoint");
                }

                ServiceUri = Path.Combine(projectPath, serviceConfig.Endpoint);
            }
            else
            {
                ServiceUri = serviceConfig.Endpoint;
            }
        }

        private async Task InitNuGetInstallerAsync()
        {
            var componentModel = await ServiceProvider.GetGlobalServiceAsync<SComponentModel, IComponentModel>();
            //TODO: use new interfaces?
            PackageInstallerServices = componentModel.GetService<IVsPackageInstallerServices>();
            PackageInstaller = componentModel.GetService<IVsPackageInstaller>();
        }

        public static async Task<T> CreateAsync<T>(ConnectedServiceHandlerContext context, Instance serviceInstance) where T : BaseCodeGenDescriptor
        {
            var instance = (T)Activator.CreateInstance(typeof(T), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, new object[] { context, serviceInstance }, null, null);
            await instance.InitializeAsync();
            return instance;
        }

        #endregion

        #region Methods

        public abstract Task AddNugetPackagesAsync();

        public abstract Task<string> AddGeneratedCodeAsync();

        public abstract Task<string> AddGeneratedNSwagFileAsync();

        protected async Task<string> GetReferenceFileFolderAsync()
        {
            var serviceReferenceFolderName = Context.HandlerHelper.GetServiceArtifactsRootFolder();
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            return Project.GetServiceFolderPath(serviceReferenceFolderName, Instance.Name);
        }

        #endregion
    }
}