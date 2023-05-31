using System.Resources;
using System.Reflection;
using System.Runtime.InteropServices;
using Unchase.OpenAPI.ConnectedService;
using Microsoft.VisualStudio.Shell;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle(Constants.ExtensionName)]
[assembly: AssemblyDescription(Constants.ExtensionDescription)]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(Constants.Author)]
[assembly: AssemblyProduct(Constants.ProviderId)]
[assembly: AssemblyCopyright(Constants.Copyright)]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.2.*")]
[assembly: AssemblyVersion(Constants.Vsix.Version)]
[assembly: AssemblyFileVersion(Constants.Vsix.Version)]
[assembly: NeutralResourcesLanguage("en-US")]

[assembly: ProvideCodeBase(AssemblyName = Constants.NewtonsoftJsonAssemblyName)]
//[assembly: ProvideBindingRedirection(AssemblyName = Constants.NewtonsoftJsonAssemblyName, OldVersionLowerBound = "10.0.0.0")]
[assembly: ProvideBindingRedirection(AssemblyName = Constants.NewtonsoftJsonAssemblyName, OldVersionLowerBound = "1.0.0.0")]
[assembly: ProvideBindingRedirection(AssemblyName = Constants.MicrosoftExtensionsFileProvidersAbstractionsAssemblyName, OldVersionLowerBound = "1.1.1.0")]
[assembly: ProvideBindingRedirection(AssemblyName = Constants.SystemRuntimeCompilerServicesUnsafeAssemblyName, OldVersionLowerBound = "0.0.0.0")]
