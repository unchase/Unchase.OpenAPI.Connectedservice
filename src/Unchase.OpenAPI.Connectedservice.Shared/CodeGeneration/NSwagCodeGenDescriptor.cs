//-----------------------------------------------------------------------
// <copyright file="NSwagCodeGenDescriptor.cs" company="Unchase">
//     Copyright (c) Nikolay Chebotov (Unchase). All rights reserved.
// </copyright>
// <license>https://github.com/unchase/Unchase.OpenAPI.Connectedservice/blob/master/LICENSE.md</license>
// <author>Nickolay Chebotov (Unchase), spiritkola@hotmail.com</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.ConnectedServices;
using Microsoft.VisualStudio.Shell;
using NSwag.Commands;
using NSwag.Commands.Generation;
using Unchase.OpenAPI.ConnectedService.Common;
using Unchase.OpenAPI.ConnectedService.Views;

namespace Unchase.OpenAPI.ConnectedService.CodeGeneration
{
    internal class NSwagCodeGenDescriptor :
        BaseCodeGenDescriptor
    {
        #region Constructors

        public NSwagCodeGenDescriptor(ConnectedServiceHandlerContext context, Instance serviceInstance)
            : base(context, serviceInstance) { }

        #endregion

        #region Methods

        #region NuGet

        public override async Task AddNugetPackagesAsync()
        {
            if (Instance.ServiceConfig.GenerateCSharpClient)
            {
                await AddCSharpClientNugetPackagesAsync();
            }

            if (Instance.ServiceConfig.GenerateCSharpController)
            {
                await AddCSharpControllerNugetPackagesAsync();
            }
        }

        internal async Task AddCSharpClientNugetPackagesAsync()
        {
            await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Adding Nuget Packages for OpenAPI (Swagger) CSharp Client...");
            const string packageSource = Constants.NuGetOnlineRepository;
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var projectTargetFrameworkMonikerFullName =
                Project.Properties.Item("TargetFrameworkMoniker").Value.ToString();
            var projectTargetFrameworkDescriptions = projectTargetFrameworkMonikerFullName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (projectTargetFrameworkDescriptions.Length == 2)
            {
                string[] nugetPackages;
                switch (projectTargetFrameworkDescriptions[0])
                {
                    case ".NETStandard":
                    case ".NETCoreApp":
                        nugetPackages = Constants.NetStandardUnsupportedVersions.Contains(projectTargetFrameworkDescriptions[1])
                            ? Array.Empty<string>()
                            : Constants.NetStandardNuGetPackages;
                        break;
                    case ".NETFramework":
                        nugetPackages = Constants.FullNetNuGetPackages;
                        break;
                    case ".NETPortable":
                        nugetPackages = Constants.PortableClassLibraryNuGetPackages;
                        break;
                    default:
                        nugetPackages = Array.Empty<string>();
                        break;
                }

                foreach (var nugetPackage in nugetPackages)
                {
                    await CheckAndInstallNuGetPackageAsync(packageSource, nugetPackage);
                }

                if (nugetPackages.Any())
                {
                    await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Nuget Packages for OpenAPI (Swagger) CSharp Client were installed.");
                }
                else
                {
                    await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Warning, $"Nuget Packages for OpenAPI (Swagger) CSharp Client was not installed for unsupported \"{projectTargetFrameworkMonikerFullName}\".");
                }
            }
            else
            {
                await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Warning, $"Nuget Packages for OpenAPI (Swagger) CSharp Client was not installed for unsupported \"{projectTargetFrameworkMonikerFullName}\".");
            }
        }

        internal async Task AddCSharpControllerNugetPackagesAsync()
        {
            await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Adding Nuget Packages for OpenAPI (Swagger) CSharp Controller...");
            const string packageSource = Constants.NuGetOnlineRepository;

            foreach (var nugetPackage in Constants.ControllerNuGetPackages)
            {
                await CheckAndInstallNuGetPackageAsync(packageSource, nugetPackage);
            }

            await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Nuget Packages for OpenAPI (Swagger) CSharp Controller were installed.");
        }

        internal async Task CheckAndInstallNuGetPackageAsync(string packageSource, string nugetPackage)
        {
            try
            {
                if (!PackageInstallerServices.IsPackageInstalled(Project, nugetPackage))
                {
                    PackageInstaller.InstallPackage(packageSource, Project, nugetPackage, (Version)null, false);
                    await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, $"Nuget Package \"{nugetPackage}\" forOpenAPI (Swagger) was added.");
                }
                else
                {
                    await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, $"Nuget Package \"{nugetPackage}\" for OpenAPI (Swagger) already installed.");
                }
            }
            catch (Exception ex)
            {
                await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Warning, $"Nuget Package \"{nugetPackage}\" for OpenAPI (Swagger) not installed. Error: {ex.Message}.");
            }
        }

        #endregion

        #region Generation

        public override async Task<string> AddGeneratedCodeAsync()
        {
            await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Generating Client Proxy for OpenAPI (Swagger) Client...");
            try
            {
                var result = await GenerateCodeAsync(Context, Instance);
                await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Client Proxy for OpenAPI (Swagger) Client was generated.");
                return result;
            }
            catch (Exception e)
            {
                await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Warning, $"Client Proxy for OpenAPI (Swagger) Client was not generated. Error: {e.Message}.");
                return string.Empty;
            }
        }

        public override async Task<string> AddGeneratedNSwagFileAsync()
        {
            await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Generating NSwag-file for OpenAPI (Swagger)...");
            try
            {
                var result = await GenerateNSwagFileAsync(Context, Instance);
                await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, $"NSwag-file \"{Path.GetFileName(result)}\" for OpenAPI (Swagger) was generated.");
                return result;
            }
            catch (Exception e)
            {
                await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Warning, $"NSwag-file for OpenAPI (Swagger) was not generated. Error: {e.Message}.");
                return string.Empty;
            }
        }

        internal async Task<string> GenerateCodeAsync(ConnectedServiceHandlerContext context, Instance instance)
        {
            var serviceFolder = instance.Name;
            var rootFolder = context.HandlerHelper.GetServiceArtifactsRootFolder();
            var folderPath = context.ProjectHierarchy.GetProject().GetServiceFolderPath(rootFolder, serviceFolder);

            var nSwagFilePath = Path.Combine(folderPath, $"{instance.ServiceConfig.GeneratedFileName}.nswag");
            var document = await NSwagDocument.LoadWithTransformationsAsync(nSwagFilePath, instance.ServiceConfig.Variables);
            document.Runtime = instance.ServiceConfig.Runtime;

            var nSwagJsonTempFileName = Path.GetTempFileName();
            var csharpClientTempFileName = Path.GetTempFileName();
            var typeScriptClientTempFileName = Path.GetTempFileName();
            var controllerTempFileName = Path.GetTempFileName();
            var nSwagJsonOutputPath = document.SelectedSwaggerGenerator.OutputFilePath;
            try
            {
                var csharpClientOutputPath = document.CodeGenerators?.OpenApiToCSharpClientCommand?.OutputFilePath;
                var typeScriptClientOutputPath = document.CodeGenerators?.OpenApiToTypeScriptClientCommand?.OutputFilePath;
                var controllerOutputPath = document.CodeGenerators?.OpenApiToCSharpControllerCommand?.OutputFilePath;

                document.SelectedSwaggerGenerator.OutputFilePath = nSwagJsonTempFileName;
                if (document.CodeGenerators?.OpenApiToCSharpClientCommand != null)
                {
                    document.CodeGenerators.OpenApiToCSharpClientCommand.OutputFilePath = csharpClientTempFileName;
                }

                if (document.CodeGenerators?.OpenApiToTypeScriptClientCommand != null)
                {
                    document.CodeGenerators.OpenApiToTypeScriptClientCommand.OutputFilePath = typeScriptClientTempFileName;
                }

                if (document.CodeGenerators?.OpenApiToCSharpControllerCommand != null)
                {
                    document.CodeGenerators.OpenApiToCSharpControllerCommand.OutputFilePath = controllerTempFileName;
                }

                await document.ExecuteAsync();

                nSwagJsonOutputPath = await context.HandlerHelper.AddFileAsync(nSwagJsonTempFileName, nSwagJsonOutputPath, new AddFileOptions { OpenOnComplete = instance.ServiceConfig.OpenGeneratedFilesOnComplete });
                if (document.CodeGenerators?.OpenApiToCSharpClientCommand != null)
                {
                    if (instance.ServiceConfig.ExcludeTypeNamesLater)
                    {
                        var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(File.ReadAllText(csharpClientTempFileName), CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest));
                        var root = await parsedSyntaxTree.GetRootAsync();
                        var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Where(c =>
                            c.AttributeLists.Any(a => a.Attributes.Any(at =>
                                at.ArgumentList.Arguments.Any(arg =>
                                    arg.GetText().ToString().Contains("\"NJsonSchema\"")))));
                        var classNames = classes.Select(c => c.Identifier.Text);
                        var excludedClasses = new CSharpClientExcludedClasses(classNames);
                        if (excludedClasses.ShowDialog() == true)
                        {
                            var excludedClassNames = excludedClasses.Classes.Where(c => c.Excluded).Select(c => c.Name).ToArray();
                            if (excludedClassNames.Any())
                            {
                                await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Regenerating client code with excluded classes...");
                                document.CodeGenerators.OpenApiToCSharpClientCommand.ExcludedTypeNames = excludedClassNames;
                                await document.ExecuteAsync();
                                await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Generating client code with excluded classes is completed.");
                            }
                        }
                    }

                    await context.HandlerHelper.AddFileAsync(csharpClientTempFileName, csharpClientOutputPath,
                        new AddFileOptions { OpenOnComplete = instance.ServiceConfig.OpenGeneratedFilesOnComplete });
                }
                if (document.CodeGenerators?.OpenApiToTypeScriptClientCommand != null)
                {
                    await context.HandlerHelper.AddFileAsync(typeScriptClientTempFileName, typeScriptClientOutputPath,
                        new AddFileOptions { OpenOnComplete = instance.ServiceConfig.OpenGeneratedFilesOnComplete });
                }

                if (document.CodeGenerators?.OpenApiToCSharpControllerCommand != null)
                {
                    await context.HandlerHelper.AddFileAsync(controllerTempFileName, controllerOutputPath,
                        new AddFileOptions { OpenOnComplete = instance.ServiceConfig.OpenGeneratedFilesOnComplete });
                }
            }
            catch (Exception ex)
            {
                await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Warning, $"Error: {ex.Message}.");
            }
            finally
            {
                if (File.Exists(nSwagJsonTempFileName))
                {
                    File.Delete(nSwagJsonTempFileName);
                }

                if (File.Exists(csharpClientTempFileName))
                {
                    File.Delete(csharpClientTempFileName);
                }

                if (File.Exists(typeScriptClientTempFileName))
                {
                    File.Delete(typeScriptClientTempFileName);
                }

                if (File.Exists(controllerTempFileName))
                {
                    File.Delete(controllerTempFileName);
                }
            }

            return nSwagJsonOutputPath;
        }

        internal async Task<string> GenerateNSwagFileAsync(ConnectedServiceHandlerContext context, Instance instance)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var nameSpace = context.ProjectHierarchy.GetProject().GetNameSpace();

            string serviceUrl;
            if (instance.ServiceConfig.UseRelativePath)
            {
                var projectPath = context.ProjectHierarchy?.GetProject().Properties.Item("FullPath").Value.ToString();
                if (projectPath == null || !File.Exists(Path.Combine(projectPath, instance.ServiceConfig.Endpoint)))
                {
                    throw new ArgumentException("Please input the service endpoint with exists file path.", "Service Endpoint");
                }

                serviceUrl = Path.Combine(projectPath, instance.ServiceConfig.Endpoint);
            }
            else
            {
                serviceUrl = instance.ServiceConfig.Endpoint;
            }

            if (string.IsNullOrWhiteSpace(instance.Name))
            {
                instance.Name = Constants.DefaultServiceName;
            }

            var rootFolder = context.HandlerHelper.GetServiceArtifactsRootFolder();
            var serviceFolder = instance.Name;
            var document = NSwagDocument.Create();
            if (instance.ServiceConfig.GenerateCSharpClient)
            {
                instance.ServiceConfig.OpenApiToCSharpClientCommand.OutputFilePath = $"{instance.ServiceConfig.GeneratedFileName}.cs";
                if (string.IsNullOrWhiteSpace(instance.ServiceConfig.OpenApiToCSharpClientCommand.Namespace))
                {
                    instance.ServiceConfig.OpenApiToCSharpClientCommand.Namespace = $"{nameSpace}.{serviceFolder}";
                }

                document.CodeGenerators.OpenApiToCSharpClientCommand = instance.ServiceConfig.OpenApiToCSharpClientCommand;
            }

            if (instance.ServiceConfig.GenerateTypeScriptClient)
            {
                instance.ServiceConfig.OpenApiToTypeScriptClientCommand.OutputFilePath = $"{instance.ServiceConfig.GeneratedFileName}.ts";
                document.CodeGenerators.OpenApiToTypeScriptClientCommand = instance.ServiceConfig.OpenApiToTypeScriptClientCommand;
            }

            if (instance.ServiceConfig.GenerateCSharpController)
            {
                instance.ServiceConfig.OpenApiToCSharpControllerCommand.OutputFilePath =
                    instance.ServiceConfig.GenerateCSharpClient
                        ? $"{instance.ServiceConfig.GeneratedFileName}Controller.cs"
                        : $"{instance.ServiceConfig.GeneratedFileName}.cs";

                if (string.IsNullOrWhiteSpace(instance.ServiceConfig.OpenApiToCSharpControllerCommand.Namespace))
                {
                    instance.ServiceConfig.OpenApiToCSharpControllerCommand.Namespace = $"{nameSpace}.{serviceFolder}";
                }

                document.CodeGenerators.OpenApiToCSharpControllerCommand = instance.ServiceConfig.OpenApiToCSharpControllerCommand;
            }

            document.SelectedSwaggerGenerator = new FromDocumentCommand
            {
                OutputFilePath = $"{instance.ServiceConfig.GeneratedFileName}.nswag.json",
                Url = instance.ServiceConfig.ConvertFromOdata ? null : serviceUrl,
                Json = instance.ServiceConfig.CopySpecification || instance.ServiceConfig.ConvertFromOdata
                    ? File.ReadAllText(instance.SpecificationTempPath)
                    : null
            };

            var json = document.ToJson();
            var tempFileName = Path.GetTempFileName();
            File.WriteAllText(tempFileName, json);
            var targetPath = Path.Combine(rootFolder, serviceFolder, $"{instance.ServiceConfig.GeneratedFileName}.nswag");
            var nSwagFilePath = await context.HandlerHelper.AddFileAsync(tempFileName, targetPath);
            if (File.Exists(tempFileName))
            {
                File.Delete(tempFileName);
            }

            if (File.Exists(instance.SpecificationTempPath))
            {
                File.Delete(instance.SpecificationTempPath);
            }

            return nSwagFilePath;
        }

        internal async Task<string> ReGenerateCSharpFileAsync(ConnectedServiceHandlerContext context, Instance instance)
        {
            var serviceFolder = instance.Name;
            var rootFolder = context.HandlerHelper.GetServiceArtifactsRootFolder();
            var folderPath = context.ProjectHierarchy.GetProject().GetServiceFolderPath(rootFolder, serviceFolder);

            var nSwagFilePath = Path.Combine(folderPath, $"{instance.ServiceConfig.GeneratedFileName}.nswag");
            var document = await NSwagDocument.LoadWithTransformationsAsync(nSwagFilePath, instance.ServiceConfig.Variables);
            document.Runtime = instance.ServiceConfig.Runtime;
            await document.ExecuteAsync();
            return document.SelectedSwaggerGenerator.OutputFilePath;
        }

        #endregion

        #endregion
    }
}