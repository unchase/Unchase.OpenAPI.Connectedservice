//-----------------------------------------------------------------------
// <copyright file="NSwagCodeGenDescriptor.cs" company="Unchase">
//     Copyright (c) Nikolay Chebotov (Unchase). All rights reserved.
// </copyright>
// <license>https://github.com/unchase/Unchase.OpenAPI.Connectedservice/blob/master/LICENSE.md</license>
// <author>Nickolay Chebotov (Unchase), spiritkola@hotmail.com</author>
//-----------------------------------------------------------------------

using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ConnectedServices;
using NSwag.Commands;
using NSwag.Commands.SwaggerGeneration;
using Unchase.OpenAPI.ConnectedService.Common;

namespace Unchase.OpenAPI.ConnectedService.CodeGeneration
{
    internal class NSwagCodeGenDescriptor : BaseCodeGenDescriptor
    {
        public NSwagCodeGenDescriptor(ConnectedServiceHandlerContext context, Instance serviceInstance) : base(context, serviceInstance) { }

        #region NuGet
        public override async Task AddNugetPackagesAsync()
        {
            if (this.Instance.ServiceConfig.GenerateCSharpClient)
                await AddCSharpClientNugetPackages();
            if (this.Instance.ServiceConfig.GenerateCSharpController)
                await AddCSharpControllerNugetPackages();
        }

        internal async Task AddCSharpClientNugetPackages()
        {
            await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Adding Nuget Packages for OpenAPI (Swagger) CSharp Client...");
            var packageSource = Constants.NuGetOnlineRepository;
            var projectTargetFrameworkMonikerFullName =
                this.Project.Properties.Item("TargetFrameworkMoniker").Value.ToString();
            var projectTargetFrameworkDescriptions = projectTargetFrameworkMonikerFullName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (projectTargetFrameworkDescriptions.Length == 2)
            {
                string[] nugetPackages;
                switch (projectTargetFrameworkDescriptions[0])
                {
                    case ".NETStandard":
                        nugetPackages = Constants.NetStandartUnsupportedVersions.Contains(projectTargetFrameworkDescriptions[1])
                            ? new string[0]
                            : Constants.NetStandartNuGetPackages;
                        break;
                    case ".NETFramework":
                        nugetPackages = Constants.FullNetNuGetPackages;
                        break;
                    case ".NETPortable":
                        nugetPackages = Constants.PortableClassLibraryNuGetPackages;
                        break;
                    default:
                        nugetPackages = new string[0];
                        break;
                }
                foreach (var nugetPackage in nugetPackages)
                    await CheckAndInstallNuGetPackageAsync(packageSource, nugetPackage);
                if (nugetPackages.Any())
                    await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Nuget Packages for OpenAPI (Swagger) CSharp Client were installed.");
                else
                {
                    await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Warning, $"Nuget Packages for OpenAPI (Swagger) CSharp Client was not installed for unsupported \"{projectTargetFrameworkMonikerFullName}\".");
                }
            }
            else
            {
                await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Warning, $"Nuget Packages for OpenAPI (Swagger) CSharp Client was not installed for unsupported \"{projectTargetFrameworkMonikerFullName}\".");
            }
        }

        internal async Task AddCSharpControllerNugetPackages()
        {
            await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Adding Nuget Packages for OpenAPI (Swagger) CSharp Controller...");
            var packageSource = Constants.NuGetOnlineRepository;

            foreach (var nugetPackage in Constants.ControllerNuGetPackages)
                await CheckAndInstallNuGetPackageAsync(packageSource, nugetPackage);
            await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Nuget Packages for OpenAPI (Swagger) CSharp Controller were installed.");
        }

        internal async Task CheckAndInstallNuGetPackageAsync(string packageSource, string nugetPackage)
        {
            try
            {
                if (!PackageInstallerServices.IsPackageInstalled(this.Project, nugetPackage))
                {
                    PackageInstaller.InstallPackage(packageSource, this.Project, nugetPackage, (Version) null, false);
                    await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, $"Nuget Package \"{nugetPackage}\" forOpenAPI (Swagger) was added.");
                }
                else
                    await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, $"Nuget Package \"{nugetPackage}\" for OpenAPI (Swagger) already installed.");
            }
            catch (Exception ex)
            {
                await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Warning, $"Nuget Package \"{nugetPackage}\" for OpenAPI (Swagger) not installed. Error: {ex.Message}.");
            }
        }
        #endregion

        public override async Task<string> AddGeneratedCodeAsync()
        {
            await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Generating Client Proxy for OpenAPI (Swagger) Client...");
            try
            {
                var result = await GenerateCodeAsync(Context, Instance);
                await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Client Proxy for OpenAPI (Swagger) Client was generated.");
                return result;
            }
            catch (Exception e)
            {
                await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Warning, $"Client Proxy for OpenAPI (Swagger) Client was not generated. Error: {e.Message}.");
                return string.Empty;
            }
        }

        public override async Task<string> AddGeneratedNswagFileAsync()
        {
            await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Generating NSwag-file for OpenAPI (Swagger)...");
            try
            {
                var result = await GenerateNswagFileAsync(Context, Instance);
                await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, $"NSwag-file \"{Path.GetFileName(result)}\" for OpenAPI (Swagger) was generated.");
                return result;
            }
            catch (Exception e)
            {
                await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Warning, $"NSwag-file for OpenAPI (Swagger) was not generated. Error: {e.Message}.");
                return string.Empty;
            }
        }

        internal async Task<string> GenerateCodeAsync(ConnectedServiceHandlerContext context, Instance instance)
        {
            var serviceFolder = instance.Name;
            var rootFolder = context.HandlerHelper.GetServiceArtifactsRootFolder();
            var folderPath = context.ProjectHierarchy.GetProject().GetServiceFolderPath(rootFolder, serviceFolder);

            var nswagFilePath = Path.Combine(folderPath, $"{serviceFolder}.nswag");
            var document = await NSwagDocument.LoadWithTransformationsAsync(nswagFilePath, instance.ServiceConfig.Variables);
            document.Runtime = instance.ServiceConfig.Runtime;

            var nswagJsonTempFileName = Path.GetTempFileName();
            var csharpClientTempFileName = Path.GetTempFileName();
            var typeScriptClientTempFileName = Path.GetTempFileName();
            var controllerTempFileName = Path.GetTempFileName();
            var nswagJsonOutputPath = document.SelectedSwaggerGenerator.OutputFilePath;
            try
            {
                var csharpClientOutputPath = document.CodeGenerators?.SwaggerToCSharpClientCommand?.OutputFilePath;
                var typeScriptClientOutputPath = document.CodeGenerators?.SwaggerToTypeScriptClientCommand?.OutputFilePath;
                var controllerOutputPath = document.CodeGenerators?.SwaggerToCSharpControllerCommand?.OutputFilePath;

                document.SelectedSwaggerGenerator.OutputFilePath = nswagJsonTempFileName;
                if (document.CodeGenerators?.SwaggerToCSharpClientCommand != null)
                    document.CodeGenerators.SwaggerToCSharpClientCommand.OutputFilePath = csharpClientTempFileName;
                if (document.CodeGenerators?.SwaggerToTypeScriptClientCommand != null)
                    document.CodeGenerators.SwaggerToTypeScriptClientCommand.OutputFilePath = typeScriptClientTempFileName;
                if (document.CodeGenerators?.SwaggerToCSharpControllerCommand != null)
                    document.CodeGenerators.SwaggerToCSharpControllerCommand.OutputFilePath = controllerTempFileName;

                await document.ExecuteAsync();

                nswagJsonOutputPath = await context.HandlerHelper.AddFileAsync(nswagJsonTempFileName, nswagJsonOutputPath, new AddFileOptions { OpenOnComplete = instance.ServiceConfig.OpenGeneratedFilesOnComplete });
                if (document.CodeGenerators?.SwaggerToCSharpClientCommand != null)
                    await context.HandlerHelper.AddFileAsync(csharpClientTempFileName, csharpClientOutputPath, new AddFileOptions { OpenOnComplete = instance.ServiceConfig.OpenGeneratedFilesOnComplete });
                if (document.CodeGenerators?.SwaggerToTypeScriptClientCommand != null)
                    await context.HandlerHelper.AddFileAsync(typeScriptClientTempFileName, typeScriptClientOutputPath, new AddFileOptions { OpenOnComplete = instance.ServiceConfig.OpenGeneratedFilesOnComplete });
                if (document.CodeGenerators?.SwaggerToCSharpControllerCommand != null)
                    await context.HandlerHelper.AddFileAsync(controllerTempFileName, controllerOutputPath, new AddFileOptions { OpenOnComplete = instance.ServiceConfig.OpenGeneratedFilesOnComplete });
            }
            catch (Exception ex)
            {
                await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Warning, $"Error: {ex.Message}.");
            }
            finally
            {
                if (File.Exists(nswagJsonTempFileName))
                    File.Delete(nswagJsonTempFileName);
                if (File.Exists(csharpClientTempFileName))
                    File.Delete(csharpClientTempFileName);
                if (File.Exists(typeScriptClientTempFileName))
                    File.Delete(typeScriptClientTempFileName);
                if (File.Exists(controllerTempFileName))
                    File.Delete(controllerTempFileName);
            }

            return nswagJsonOutputPath;
        }

        internal async Task<string> GenerateNswagFileAsync(ConnectedServiceHandlerContext context, Instance instance)
        {
            var nameSpace = context.ProjectHierarchy.GetProject().GetNameSpace();
            var serviceUrl = instance.ServiceConfig.Endpoint;
            var rootFolder = context.HandlerHelper.GetServiceArtifactsRootFolder();
            var serviceFolder = instance.Name;
            var document = NSwagDocument.Create();
            if (instance.ServiceConfig.GenerateCSharpClient)
            {
                instance.ServiceConfig.SwaggerToCSharpClientCommand.OutputFilePath = $"{serviceFolder}Client.Generated.cs";
                if (string.IsNullOrWhiteSpace(instance.ServiceConfig.SwaggerToCSharpClientCommand.Namespace))
                    instance.ServiceConfig.SwaggerToCSharpClientCommand.Namespace = $"{nameSpace}.{serviceFolder}";
                document.CodeGenerators.SwaggerToCSharpClientCommand = instance.ServiceConfig.SwaggerToCSharpClientCommand;
            }
            if (instance.ServiceConfig.GenerateTypeScriptClient)
            {
                instance.ServiceConfig.SwaggerToTypeScriptClientCommand.OutputFilePath = $"{serviceFolder}Client.Generated.ts";
                document.CodeGenerators.SwaggerToTypeScriptClientCommand = instance.ServiceConfig.SwaggerToTypeScriptClientCommand;
            }
            if (instance.ServiceConfig.GenerateCSharpController)
            {
                instance.ServiceConfig.SwaggerToCSharpControllerCommand.OutputFilePath = $"{serviceFolder}Controller.Generated.cs";
                if (string.IsNullOrWhiteSpace(instance.ServiceConfig.SwaggerToCSharpControllerCommand.Namespace))
                    instance.ServiceConfig.SwaggerToCSharpControllerCommand.Namespace = $"{nameSpace}.{serviceFolder}";
                document.CodeGenerators.SwaggerToCSharpControllerCommand = instance.ServiceConfig.SwaggerToCSharpControllerCommand;
            }

            document.SelectedSwaggerGenerator = new FromSwaggerCommand
            {
                OutputFilePath = $"{serviceFolder}.nswag.json",
                Url = serviceUrl,
                Swagger = instance.ServiceConfig.CopySpecification ? File.ReadAllText(instance.SpecificationTempPath) : null
            };

            var json = document.ToJson();
            var tempFileName = Path.GetTempFileName();
            File.WriteAllText(tempFileName, json);
            var targetPath = Path.Combine(rootFolder, serviceFolder, $"{serviceFolder}.nswag");
            var nswagFilePath = await context.HandlerHelper.AddFileAsync(tempFileName, targetPath);
            if (File.Exists(tempFileName))
                File.Delete(tempFileName);
            if (File.Exists(instance.SpecificationTempPath))
                File.Delete(instance.SpecificationTempPath);
            return nswagFilePath;
        }

        internal async Task<string> ReGenerateCSharpFileAsync(ConnectedServiceHandlerContext context, Instance instance)
        {
            var serviceFolder = instance.Name;
            var rootFolder = context.HandlerHelper.GetServiceArtifactsRootFolder();
            var folderPath = context.ProjectHierarchy.GetProject().GetServiceFolderPath(rootFolder, serviceFolder);

            var nswagFilePath = Path.Combine(folderPath, $"{serviceFolder}.nswag");
            var document = await NSwagDocument.LoadWithTransformationsAsync(nswagFilePath, instance.ServiceConfig.Variables);
            document.Runtime = instance.ServiceConfig.Runtime;
            await document.ExecuteAsync();
            return document.SelectedSwaggerGenerator.OutputFilePath;
        }
    }
}
