# Road map

- [ ] Fix open [issues](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/issues/)
- [ ] Gather [feedback](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/issues/new) on [`Unchase OpenAPI (Swagger API) Connected Service`](https://marketplace.visualstudio.com/items?itemName=unchase.UnchaseOpenAPIConnectedService) extension for [`Visual Studio`](https://visualstudio.microsoft.com/vs/) and plan further features to implement

# Change log

These are the changes to each version that has been released on the official [Visual Studio extension gallery](https://marketplace.visualstudio.com/items?itemName=unchase.UnchaseOpenAPIConnectedService).

## v1.5.4 `(2020-08-03)`

- [x] Update dependencies: add support for [NSwag.Commands v13.7.0](https://github.com/RicoSuter/NSwag/pull/2979)

## v1.5.3 `(2020-07-23)`

- [x]  Update dependencies: add support for [OpenAPI.NET.OData v1.0.4](https://www.nuget.org/packages/Microsoft.OpenApi.OData)

## v1.5.2 `(2020-06-26)`

- [x] Update dependencies: add support for [NSwag.Commands v13.6.2](https://github.com/RicoSuter/NSwag/pull/2925)

## v1.5.1 `(2020-06-12)`

- [x] Update dependencies: add support for [NSwag.Commands v13.6.1](https://github.com/RicoSuter/NSwag/pull/2894)
  - [x] Add NewLineBehaviour
  - [x] Fix enum style UI
  - [x] Add default CancellationToken parameter to C# controller template
  - [x] Update NJS, add GenerateNullableReferenceTypes to CLI/UI
- [x] Update `ps1`-script for publishing vsix to vsixgallery.com
- [x] Fix bug: add some missed C# client and controller generation options

## v1.5.0 `(2020-06-05)`

- [x] Add experimental feature: ability to convert to OpenAPI specification from OData specification based on [OpenAPI.NET.OData](https://github.com/microsoft/OpenAPI.NET.OData) before code generation

## v1.4.8 `(2020-05-29)`

- [x] Update dependencies: add support for [NSwag.Commands v13.6.0](https://github.com/RicoSuter/NSwag/pull/2860)
  - [x] [Restore status validation, allow error response interception (axios)](https://github.com/RicoSuter/NSwag/pull/2838)
  - [x] [Add support for axios cancellation token](https://github.com/RicoSuter/NSwag/pull/2846)
  - [x] Improve form data support (OpenAPI 3)
  - [x] Improve form data handling

## v1.4.7 `(2020-05-09)`

- [x] Update dependencies: add support for [NSwag.Commands v13.5.0](https://github.com/RicoSuter/NSwag/pull/2837)

## v1.4.6 `(2020-04-18)`

- [x] Update dependencies: add support for [NSwag.Commands v13.4.2](https://github.com/RicoSuter/NSwag/pull/2808)
  - [x] Fix Axios exception handling

## v1.4.5 `(2020-04-16)`

- [x] Update dependencies: add support for [NSwag.Commands v13.4.1](https://github.com/RicoSuter/NSwag/pull/2800)

## v1.4.4 `(2020-04-16)`

- [x] Update dependencies: add support for [NSwag.Commands v13.4.0](https://github.com/RicoSuter/NSwag/pull/2799)
  - [x] [New option: define a base interface for C# client interfaces.](https://github.com/RicoSuter/NSwag/pull/2775)
  - [x] [Use enums value (underlying type) rather than enum name](https://github.com/RicoSuter/NSwag/pull/2758)
  - [x] Inline param refs if AllowReferencesWithProperties is set
  - [x] Removed .NET Core 1.0, 1.1 and 2.0 support.
- [x] Add feature: Add option to set `Genereated file name` on the first wizard page
- [x] Disable some fields when updating

## v1.4.3 `(2020-04-10)`

- [x] Fix [issue #18](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/issues/18)

## v1.4.2 `(2020-03-21)`

- [x] Update dependencies: add support for [NSwag.Commands v13.3.0](https://github.com/RicoSuter/NSwag/pull/2739)

## v1.4.1 `(2020-03-06)`

- [x] Update dependencies: add support for [NSwag.Commands v13.2.4](https://github.com/RicoSuter/NSwag/pull/2719)
  - [x] Add `EnumStyle` to command and ui
- [x] Update dependencies: add support for [NSwag.Commands v13.2.5](https://github.com/RicoSuter/NSwag/pull/2720)

## v1.4.0 `(2020-02-27)`

- [x] Add feature: exclude type names in separate Window for C# client code generation

## v1.3.18 `(2020-02-13)`

- [x] Update dependencies: add support for [NSwag.Commands v13.2.3](https://github.com/RicoSuter/NSwag/pull/2661)
  - [x] [Add title, description and version to C# client template model](https://github.com/RicoSuter/NSwag/pull/2665)
  - [x] [Add support for custom inline styles for Swagger UI](https://github.com/RicoSuter/NSwag/pull/2666)
  - [x] [Add code generated attribute to file response](https://github.com/RicoSuter/NSwag/pull/2669)
  - [x] [Add support for custom document title and head content](https://github.com/RicoSuter/NSwag/pull/2678)
  - [x] [Enable using route name as operationId](https://github.com/RicoSuter/NSwag/pull/2681)
  - [x] [Fixes an issue where GET method parameter becomes BODY parameter](https://github.com/RicoSuter/NSwag/pull/2683)

## v1.3.17 `(2020-01-17)`

- [x] Update dependencies: add support for [NSwag.Commands v13.2.2](https://github.com/RicoSuter/NSwag/pull/2634)

## v1.3.16 `(2020-01-13)`

- [x] Update dependencies: add support for [NSwag.Commands v13.2.1](https://github.com/RicoSuter/NSwag/pull/2620)

## v1.3.15 `(2019-12-19)`

- [x] Update dependencies: add support for [NSwag.Commands v13.2.0](https://github.com/RicoSuter/NSwag/pull/2588)

## v1.3.14 `(2019-12-17)`

- [x] Fix [issue #12](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/issues/12)

## v1.3.13 `(2019-11-23)`

- [x] Update dependencies: add support for [NSwag.Commands v13.1.6](https://github.com/RicoSuter/NSwag/pull/2543)
  - [x] [Update NJS](https://github.com/RicoSuter/NSwag/commit/ef1cd8781c5a90e58045cf90e367a68e3ca70408)
  - [x] [Add `OpenApiBodyParameterAttribute`](https://github.com/RicoSuter/NSwag/commit/7eca48175cf6df3564059df3c91aa61a4a8b25af)
  - [x] [Allow angular http request to send withCredentials](https://github.com/RicoSuter/NSwag/pull/2538)

## v1.3.10 `(2019-11-22)`

- [x] Fix [issue #10](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/issues/10)
- [x] Add small fixes 

## v1.3.9 `(2019-11-14)`

- [x] Add output relative project path in the first wizard page

## v1.3.8 `(2019-11-14)`

- [x] Fix [issue #9](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/issues/9)
- [x] Update dependencies: add support for [NSwag.Commands v13.1.4](https://github.com/RicoSuter/NSwag/pull/2520) and [NSwag.Commands v13.1.5](https://github.com/RicoSuter/NSwag/pull/2523)

## v1.3.7 `(2019-10-20)`

- [x] Update icon resolution

## v1.3.6 `(2019-10-10)`

- [x] Fix [issue #6](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/issues/6)
- [x] Update dependencies: add support for [NSwag.Commands v13.1.3](https://github.com/RicoSuter/NSwag/pull/2454)

## v1.3.5 `(2019-09-28)`

- [x] Update dependencies: add support for [NSwag.Commands v13.1.2](https://github.com/RicoSuter/NSwag/pull/2433)

## v1.3.4 `(2019-09-27)`

- [x] Update dependencies: add support for [NSwag.Commands v13.1.1](https://github.com/RicoSuter/NSwag/pull/2430)
  - [x] Add .NET Core 3.0 support
  - [x] Improve JSON Schema generator fallback for System.Text.Json (still not recommended)
- [x] Update other nuget-dependencies

## v1.3.3 `(2019-09-25)`

- [x] Add "Browse" button in the first wizard page for choosing specification file

## v1.3.2 `(2019-09-06)`

- [x] Update dependencies: add support for [NSwag.Commands v13.0.6](https://github.com/RicoSuter/NSwag/pull/2369)

## v1.3.0 `(2019-06-29)`

- [x] Update dependencies: add support for [NSwag.Commands v13.0.3](https://github.com/RicoSuter/NSwag/pull/2277)
- [x] Add `Inline named arays`, `Inline named dictonaries`, `Inline named tuples` and `Inline named any schemas` options to `CSharp Client Settings` and `CSharp Controller Settings` wizard pages
- [x] Add `Add model validation attributes` option to `CSharp Controller Settings` wizard page
- [x] Add `Any Type` textbox into `Primitive Types` group to `CSharp Client Settings` and `CSharp Controller Settings` wizard pages
- [x] Add `Inline named dictonaries` and `Inline named any schemas` options to `TypeScript Client Settings` wizard page

## v1.2.14 `(2019-06-23)`

- [x] Add support yaml-endpoints and endpoints not ends with ".json" or ".yml"

## v1.2.10 `(2019-05-22)`

- [x] Update dependencies: add support for [NSwag.Commands v12.3.1](https://github.com/RicoSuter/NSwag/pull/2187)

## v1.2.9 `(2019-05-22)`

- [x] Update dependencies: add support for [NSwag.Commands v12.3.0](https://www.nuget.org/packages/NSwag.Commands/)

## v1.2.6 `(2019-05-01)`

- [x] Update dependencies: add support for [NSwag.Commands v12.2.4](https://github.com/RicoSuter/NSwag/pull/2144)

## v1.2.4 `(2019-04-30)`

- [x] Add a button in the first wizard page for reporting bugs in github project [issues](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/issues)

## v1.2.1 `(2019-04-22)`

- [x] Update dependencies: add support for [NSwag.Commands v12.2.0](https://github.com/RicoSuter/NSwag/pull/2128)

## v1.2.0 `(2019-04-20)`

- [x] Add `Compare OpenAPI-specifications... Command` section to [`README`](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/)
- [x] Add menu command embedded in Visual Studio Solution Explorer context menu lets you compare generated `.nswag.json` specification-file with another `.nswag.json` specification-file (or with specification given by `endpoint`)
- [x] Fix bug with empty `ServiceName`
- [x] Small fixes

## v1.1.15 `(2019-04-20)`

- [x] Add fix: service now visible in `VisualBasic` projects
- [x] Add small fix: Fix service "ExtensionName"

## v1.1.14 `(2019-04-19)`

- [x] Add fix: Add the ability to automatically install required nuget-packages for .net core

## v1.1.13 `(2019-04-16)`

- [x] Add bug fix: Service update no longer blocks input fields for `ServiceName` and `Endpoint` (if you added several endpoints and want to update one of them).

## v1.1.12 `(2019-04-16)`

- [x] Add fix: Updating the service blocks the input field for the `ServiceName`.

## v1.1.9 `(2019-04-14)`

- [x] Change [`LICENSE`](LICENSE.md): [MIT License](https://mit-license.org) to [Apache 2.0 License](http://www.apache.org/licenses/LICENSE-2.0).

## v1.1.7 `(2019-04-14)`

- [x] Fix: When updating a connected service: the configuration is loaded from the last saved configuration; not the entire first page of the wizard is blocked, but only the `Endpoint`.
- [x] Update [`README`](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/)
- [x] Minor fixes

## v1.1.6 `(2019-04-11)`

- [x] Add: in the first page of wizard add checkbox for open generated files on complete
- [x] Fix: add `TextWrapping` for correct display checkboxes text in wizard pages
- [x] Fix bug: now generated client (controller) code files add into the project

## v1.1.5 `(2019-04-10)`

- [x] Small fixes

## v1.1.4 `(2019-04-10)`

- [x] Delete `Visual Studio MPF 15.0` from service Dependencies

## v1.1.0 `(2019-04-10)`

- [x] Add `Open in NSwagStudio Command` section to [`README`](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/)
- [x] Add menu command embedded in Visual Studio Solution Explorer context menu lets you open generated `.nswag` and `.nswag.json` files in [NSWagStudio](https://github.com/NSwag/NSwag/wiki/NSwagStudio)

## v1.0.6 `(2019-04-08)`

- [x] Add ability to hide/show "Runtime and variables" settings in the first page of service settings wizard

## v1.0.5 `(2019-04-08)`

- [x] Storage last OpenAPI (Swagger) specification *Endpoint* and *Service name*

## v1.0.0 `(2019-04-07)`

- [x] Add required dependencies for the `C#` client 
- [x] Generate `C#` `Controller` code and `C#` (`TypeScript`) `HttpClient` code with [NSwag](https://github.com/RSuter/NSwag) - a `Swagger/OpenAPI` 2.0 and 3.0 toolchain
- [x] Add `Settings Meaning` section to [`README`](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/)
- [x] Add `Troubleshooting` section to [`README`](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/)
- [x] Add `Getting Started` overview to GitHub [`README`](https://github.com/unchase/Unchase.OpenAPI.ConnectedService/)
- [x] Release version of `Unchase OpenAPI (Swagger API) Connected Service` extension for VS2017/2019 on [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=unchase.UnchaseOpenAPIConnectedService) 
