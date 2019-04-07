using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Unchase.OpenAPI.ConnectedService.Common
{
    /// <summary>
    /// A utility class for working with Visual Studio project system.
    /// </summary>
    internal static class ProjectHelper
    {
        public const int __VSHPROPID_VSHPROPID_ExtObject = -2027;

        public static Project GetProject(this IVsHierarchy projectHierarchy)
        {
        
        int result = projectHierarchy.GetProperty(
                VSConstants.VSITEMID_ROOT,
                __VSHPROPID_VSHPROPID_ExtObject, //(int)__VSHPROPID.VSHPROPID_ExtObject,
                out object projectObject);
            ErrorHandler.ThrowOnFailure(result);
            return (Project)projectObject;
        }

        public static string GetNameSpace(this Project project)
        {
            return project.Properties.Item("DefaultNamespace").Value.ToString();
        }

        public static string GetServiceFolderPath(this Project project, string rootFolder, string serviceName)
        {
            var servicePath = project.ProjectItems
                .Item(rootFolder).ProjectItems
                .Item(serviceName).Properties
                .Item("FullPath").Value.ToString() ?? project.Properties.Item("FullPath").Value.ToString();

            return servicePath;
        }
    }
}
