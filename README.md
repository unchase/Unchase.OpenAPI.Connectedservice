<p align="center">
  <a href="https://marketplace.visualstudio.com/items?itemName=Unchase.unchaseopenapiconnectedservice">
    <img src="img/Unchase-OpenAPI-Swagger-Connected-Service-Logo.png" alt="Unchase OpenAPI (Swagger) Connected Service Logo">
  </a>
</p>

[Unchase OpenAPI (Swagger) Connected Service](https://marketplace.visualstudio.com/items?itemName=Unchase.unchaseOpenAPIConnectedService) is a Visual Studio 2017/2019 extension to generate `C#` (`TypeScript`) `HttpClient` (or `C#` `Controllers`) code for `OpenAPI` (formerly [`Swagger API`](https://swagger.io/docs/specification/about/)) web service with [NSwag](https://github.com/RSuter/NSwag).

> Starting from Visual Studio Community 2019 v16.1.3 extensions based on `Microsoft Connected Services` now work fine.

> The project is developed and maintained by [Nikolay Chebotov (**Unchase**)](https://github.com/unchase).

## Getting Started

#### **[Read How-To on medium.com](https://medium.com/@unchase/how-to-generate-c-or-typescript-client-code-for-openapi-swagger-specification-d882d59e3b77)**

Install from `Tools -> Extensions and Updates` menu inside [Visual Studio](https://visualstudio.microsoft.com/vs/) 2017 (for [VisualStudio](https://visualstudio.microsoft.com/vs/) 2019: `Extensions -> Manage Extensions`) or [download](http://vsixgallery.com/extensions/Unchase.OpenAPI.ConnectedService.63199638-6211-4285-ba8f-75b1f0326c2a/extension.vsix)  as `VSIX` package from VSGallery or [download](https://marketplace.visualstudio.com/items?itemName=unchase.unchaseOpenAPIConnectedService)  as `VSIX` package from [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=Unchase.unchaseopenapiconnectedservice):

![Adding Unchase OpenAPI (Swagger) Connected Service in Visual Studio](img/Unchase-OpenAPI-Swagger-Connected-Service.gif)

## Builds status

|Status|Value|
|:----|:---:|
|Build|[![Build status](https://ci.appveyor.com/api/projects/status/90oewanfh32fjcr6)](https://ci.appveyor.com/project/unchase/unchase.openapi.connectedservice)
|Buid History|![Build history](https://buildstats.info/appveyor/chart/unchase/unchase-openapi-connectedservice)
|GitHub Release|[![GitHub release](https://img.shields.io/github/release/unchase/Unchase.OpenAPI.Connectedservice.svg)](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/releases/latest)
|GitHub Release Date|[![GitHub Release Date](https://img.shields.io/github/release-date/unchase/Unchase.OpenAPI.Connectedservice.svg)](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/releases/latest)
|GitHub Release Downloads|[![Github Releases](https://img.shields.io/github/downloads/unchase/Unchase.OpenAPI.Connectedservice/total.svg)](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/releases/latest)
|VS Marketplace|[![VS Marketplace](http://vsmarketplacebadge.apphb.com/version-short/unchase.UnchaseOpenAPIConnectedService.svg)](https://marketplace.visualstudio.com/items?itemName=unchase.unchaseOpenAPIConnectedService)
|VS Marketplace Downloads|[![VS Marketplace Downloads](http://vsmarketplacebadge.apphb.com/downloads-short/unchase.UnchaseOpenAPIConnectedService.svg)](https://marketplace.visualstudio.com/items?itemName=unchase.unchaseOpenAPIConnectedService)
|VS Marketplace Installs|[![VS Marketplace Installs](http://vsmarketplacebadge.apphb.com/installs-short/unchase.UnchaseOpenAPIConnectedService.svg)](https://marketplace.visualstudio.com/items?itemName=unchase.unchaseOpenAPIConnectedService)

## Features

- Generate `C#` or `TypeScript` clients/proxies (client code) from Swagger 2.0 and OpenAPI 3.0 specifications
- Generate `C#` ASP.NET Controller from Swagger 2.0 and OpenAPI 3.0 specifications
- Generate `.nswag` file for using in [`NSwagStudio`](https://github.com/NSwag/NSwag/wiki/NSwagStudio) (no need to install for generating)
- Add required dependencies for the `C#` client (before generating):
	- Library targeting .NET Standard 1.4+:
		1. Newtonsoft.Json ([NuGet](https://www.nuget.org/packages/Newtonsoft.Json))
		2. System.Net.Http ([NuGet](https://www.nuget.org/packages/System.Net.Http))
		3. System.ComponentModel.Annotations ([NuGet](https://www.nuget.org/packages/System.ComponentModel.Annotations))
	- Library targeting the full .NET:
		1. Newtonsoft.Json ([NuGet](https://www.nuget.org/packages/Newtonsoft.Json))
		2. System.Runtime.Serialization (GAC)
		3. System.ComponentModel.DataAnnotations (GAC)
	- Library targeting PCL 259 (Portable Class Library):
		1. Newtonsoft.Json ([NuGet](https://www.nuget.org/packages/Newtonsoft.Json))
		2. Microsoft.Net.Http ([NuGet](https://www.nuget.org/packages/Microsoft.Net.Http))
		3. Portable.DataAnnotations ([NuGet](https://www.nuget.org/packages/Portable.DataAnnotations))
- Add Required dependences for the `C#` controller (before generating):
	1. Microsoft.AspNetCore.Mvc ([NuGet](https://www.nuget.org/packages/Microsoft.AspNetCore.MVC))
- **Command** to open generated `.nswag` and `.nswag.json` files in [NSWagStudio](https://github.com/NSwag/NSwag/wiki/NSwagStudio)
- **Command** to compare `.nswag.json` specification file with another `.nswag.json` specification file (or specification given by `endpoint`)
- Storage of the last 10 endpoints (specification path)

## Settings Meaning

Meaning of the Unchase [OpenAPI (Swagger) Connected Service](https://marketplace.visualstudio.com/items?itemName=unchase.unchaseOpenAPIConnectedService) settings according to [NSwagStudio](https://github.com/NSwag/NSwag/wiki/NSwagStudio):

![Unchase OpenAPI (Swagger) Connected Service settings meaning](img/Unchase-OpenAPI-Swagger-Connected-Service-Settings-Meaning.png)

## Exclude type names

Since [v1.4.0](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/releases/tag/v1.4.0) you can exclude type names in separate Window for C# client code generation:

![Unchase OpenAPI (Swagger) Connected Service - exclude type names](img/Csharp-Client-Generation-Exclude-Type-Names.png)

## Custom Commands

### `Open in NSwagStudio` Command

Since *v1.1.** have been added menu command embedded in Visual Studio Solution Explorer context menu lets you open generated `.nswag` and `.nswag.json` files in [NSwagStudio](https://github.com/NSwag/NSwag/wiki/NSwagStudio).

This extension is for those times where you generate `.nswag` and `.nswag.json` files and you want to be able to quickly open it in [NSwagStudio](https://github.com/NSwag/NSwag/wiki/NSwagStudio).

#### Prerequisite

> In order to use this extension, you must have [Visual Studio](https://visualstudio.microsoft.com/vs/) 2017/2019, this connected service as well as [NSwagStudio](https://github.com/NSwag/NSwag/wiki/NSwagStudio) installed.

#### Solution Explorer

You can open `.nswag` and `.nswag.json` files in [NSWagStudio](https://github.com/NSwag/NSwag/wiki/NSwagStudio) by simply right-clicking it in Solution Explorer and select **Open in NSwagStudio**:

![Open in NSwagStudio menu Command](img/OpenWithNSwagCommandMenu.png)

#### Path to NSwagStudio.exe

If you installed [NSwagStudio](https://github.com/NSwag/NSwag/wiki/NSwagStudio) at a non-default location, a prompt will ask for the path to `NSwagStudio.exe`.

You can always change the location in *Tools -> Options -> Web -> Unchase OpenAPI (Swagger) Connected Service*:

![Open in NSwagStudio Option](img/UnchaseOpenAPIConnectedServiceCommandsOptions1.png)

### `Compare OpenAPI-specifications...` Command

Since *v1.2.** have been added menu command embedded in Visual Studio Solution Explorer context menu lets you compare generated `.nswag.json` specification-file with another `.nswag.json` specification-file (or with specification given by `endpoint`).

This extension is for those times where you generate `.nswag.json` file and you want to quickly compare it with another specification or specification given by `endpoint`.

#### Prerequisite

> In order to use this extension, you must have [Visual Studio](https://visualstudio.microsoft.com/vs/) 2017/2019 as well as this connected service.

#### Solution Explorer

You can compare `.nswag.json` specification-file with another `.nswag.json` specification-file (or with specification given by `endpoint`) by simply selecting one or two files and right-clicking them in Solution Explorer and select **Compare OpenAPI-specifications...**:

![Compare OpenAPI Specifications Command](img/CompareOpenAPISpecificationsOneFileCommandMenu.png) ![Compare OpenAPI Specifications Command](img/CompareOpenAPISpecificationsTwoFilesCommandMenu.png)

#### Path to the specification `Endpoint`

You can always change the specification Endpoint to compare with in *Tools -> Options -> Web -> Unchase OpenAPI (Swagger) Connected Service*:

![Compare OpenAPI Specifications Option](img/UnchaseOpenAPIConnectedServiceCommandsOptions2.png)

#### Compare View

![Compare OpenAPI Specifications Command result](img/CompareOpenAPISpecificationsCommandResult.png)

## HowTos

- [ ] Add HowTos in a future
- [ ] ... [request for HowTo you need](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/issues/new?title=DOC)

## Troubleshooting

### Can't open .nswag file in NSwagStudio

- You can use **Open in NSwagStudio** menu command
- If generated code corrupted, try to open `.nswag` file in [`NSwagStudio`](https://github.com/RSuter/NSwag/wiki/NSwagStudio) (Windows GUI for editing .*nswag files)
- If it doesn't open, try to create new `.nswag` file in [`NSwagStudio`](https://github.com/RSuter/NSwag/wiki/NSwagStudio) for the same API service link and check the differences

### Installation completes but I can't see the Service in the list of connected services (Visual Studio 2019)

- Relevant [bug report](https://developercommunity.visualstudio.com/content/problem/468751/vs2019-preview-cannot-install-connected-service-ex.html). `Connected Services` restored in the v16.1.3 update to [Visual Studio](https://visualstudio.microsoft.com/vs/) 2019.

## Roadmap

See the [changelog](CHANGELOG.md) for the further development plans and version history.

## Feedback

Please feel free to add your [review](https://marketplace.visualstudio.com/items?itemName=unchase.unchaseOpenAPIConnectedService&ssr=false#review-details), [request a feature](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/issues/new?title=FEATURE), [ask a question](https://marketplace.visualstudio.com/items?itemName=unchase.unchaseOpenAPIConnectedService&ssr=false#qna) or [report a bug](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/issues/new?title=BUG) including in connected service: 

![Unchase OpenAPI Connected Service Report a Bug](img/Unchase-OpenAPI-Connected-Service-ReportBug.png)

Thank you in advance!

## Thank me!

If you like what I am doing and you would like to thank me, please consider:

[![Buy me a coffe!](img/buymeacoffe.png)](https://www.buymeacoffee.com/nikolaychebotov)

Thank you for your support!

----------

Copyright &copy; 2019 [Nikolay Chebotov (**Unchase**)](https://github.com/unchase) - Provided under the [Apache License 2.0](LICENSE.md).

