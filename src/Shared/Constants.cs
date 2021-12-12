namespace Unchase.OpenAPI.ConnectedService
{
    public static class Constants
    {
        public static class Vsix
        {
            public const string Version = "1.7.0";
        }

        public const string Author = "Nikolay Chebotov (Unchase)";
        public const string ExtensionCategory = "OpenAPI";
        public const string ExtensionName = "Unchase OpenAPI (Swagger) Connected Service";
        public const string ExtensionDescription = "Generates C# HttpClient code for OpenAPI (Swagger API) web service with NSwag.";
        public const string ProviderId = "Unchase.OpenAPI.ConnectedService";
        public const string Website = "https://github.com/unchase/Unchase.OpenAPI.Connectedservice/";
        public const string Copyright = "Copyright © 2021";

        public const string NuGetOnlineRepository = "https://www.nuget.org/api/v2/";
        public const string DefaultServiceName = "OpenAPIService";
        public const string DefaultGeneratedFileName = "OpenAPI";

        public const string NewtonsoftJsonNuGetPackage = "Newtonsoft.Json";
        public const string SystemNetHttpNuGetPackage = "System.Net.Http";
        public const string SystemComponentModelAnnotationsNuGetPackage = "System.ComponentModel.Annotations";
        public const string PortableDataAnnotationsNuGetPackage = "Portable.DataAnnotations";
        public const string MicrosoftAspNetCoreMvcNuGetPackage = "Microsoft.AspNetCore.Mvc";

        public const string OpenApiConvertSettingsPathPrefix = "OData";

        public static string[] NetStandardNuGetPackages = {
            NewtonsoftJsonNuGetPackage,
            SystemNetHttpNuGetPackage,
            SystemComponentModelAnnotationsNuGetPackage
        };

        public static string[] FullNetNuGetPackages = {
            NewtonsoftJsonNuGetPackage
        };

        public static string[] PortableClassLibraryNuGetPackages = {
            NewtonsoftJsonNuGetPackage,
            SystemNetHttpNuGetPackage,
            PortableDataAnnotationsNuGetPackage
        };

        public static string[] ControllerNuGetPackages =
        {
            MicrosoftAspNetCoreMvcNuGetPackage
        };

        public static string[] NetStandardUnsupportedVersions =
        {
            "Version=v1.0",
            "Version=v1.1",
            "Version=v1.2",
            "Version=v1.3"
        };
    }
}
