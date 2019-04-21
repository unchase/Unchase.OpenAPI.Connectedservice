# Road map

- [ ] Fix open [issues](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/issues/)
- [ ] Gather [feedback](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/issues/new) on [`Unchase OpenAPI (Swagger API) Connected Service`](https://marketplace.visualstudio.com/items?itemName=unchase.UnchaseOpenAPIConnectedService) extension for [`Visual Studio`](https://visualstudio.microsoft.com/vs/) and plan further features to implement

# Change log

These are the changes to each version that has been released on the official [Visual Studio extension gallery](https://marketplace.visualstudio.com/items?itemName=unchase.UnchaseOpenAPIConnectedService).

## v1.2.1 `(2019-04-22)`

- [x] Updated dependencies: added support for [NSwgag.Commands v12.2.0](https://www.nuget.org/packages/NSwag.Commands/)

## v1.2.0 `(2019-04-20)`

- [x] Added `Compare OpenAPI-specifications... Command` section to [`README`](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/)
- [x] Added menu command embedded in Visual Studio Solution Explorer context menu lets you compare generated `.nswag.json` specification-file with another `.nswag.json` specification-file (or with specification given by `endpoint`)
- [x] Fixed bug with empty `ServiceName`
- [x] Small fixes

## v1.1.15 `(2019-04-20)`

- [x] Added fix: service now visible in `VisualBasic` projects
- [x] Added small fix: Fixed service "ExtensionName"

## v1.1.14 `(2019-04-19)`

- [x] Added fix: Added the ability to automatically install required nuget-packages for .net core

## v1.1.13 `(2019-04-16)`

- [x] Added bug fix: Service update no longer blocks input fields for `ServiceName` and `Endpoint` (if you added several endpoints and want to update one of them).

## v1.1.12 `(2019-04-16)`

- [x] Added fix: Updating the service blocks the input field for the `ServiceName`.

## v1.1.9 `(2019-04-14)`

- [x] Changed [`LICENSE`](LICENSE.md): [MIT License](https://mit-license.org) to [Apache 2.0 License](http://www.apache.org/licenses/LICENSE-2.0).

## v1.1.7 `(2019-04-14)`

- [x] Fixed: When updating a connected service: the configuration is loaded from the last saved configuration; not the entire first page of the wizard is blocked, but only the `Endpoint`.
- [x] Updated [`README`](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/)
- [x] Minor fixes

## v1.1.6 `(2019-04-11)`

- [x] Added: in the first page of wizard added checkbox for open generated files on complete
- [x] Fix: add `TextWrapping` for correct display checkboxes text in wizard pages
- [x] Fix bug: now generated client (controller) code files add into the project

## v1.1.5 `(2019-04-10)`

- [x] Small fixes

## v1.1.4 `(2019-04-10)`

- [x] Deleted `Visual Studio MPF 15.0` from service Dependencies

## v1.1.0 `(2019-04-10)`

- [x] Added `Open in NSwagStudio Command` section to [`README`](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/)
- [x] Added menu command embedded in Visual Studio Solution Explorer context menu lets you open generated `.nswag` and `.nswag.json` files in [NSWagStudio](https://github.com/NSwag/NSwag/wiki/NSwagStudio)

## v1.0.6 `(2019-04-08)`

- [x] Added ability to hide/show "Runtime and variables" settings in the first page of service settings wizard

## v1.0.5 `(2019-04-08)`

- [x] Storage last OpenAPI (Swagger) specification *Endpoint* and *Service name*

## v1.0.0 `(2019-04-07)`

- [x] Add required dependencies for the `C#` client 
- [x] Generate `C#` `Controller` code and `C#` (`TypeScript`) `HttpClient` code with [NSwag](https://github.com/RSuter/NSwag) - a `Swagger/OpenAPI` 2.0 and 3.0 toolchain
- [x] Add `Settings Meaning` section to [`README`](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/)
- [x] Add `Troubleshooting` section to [`README`](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/)
- [x] Add `Getting Started` overview to GitHub [`README`](https://github.com/unchase/Unchase.OpenAPI.ConnectedService/)
- [x] Release version of `Unchase OpenAPI (Swagger API) Connected Service` extension for VS2017/2019 on [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=unchase.UnchaseOpenAPIConnectedService) 
