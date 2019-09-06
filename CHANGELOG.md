# Road map

- [ ] Fix open [issues](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/issues/)
- [ ] Gather [feedback](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/issues/new) on [`Unchase OpenAPI (Swagger API) Connected Service`](https://marketplace.visualstudio.com/items?itemName=unchase.UnchaseOpenAPIConnectedService) extension for [`Visual Studio`](https://visualstudio.microsoft.com/vs/) and plan further features to implement

# Change log

These are the changes to each version that has been released on the official [Visual Studio extension gallery](https://marketplace.visualstudio.com/items?itemName=unchase.UnchaseOpenAPIConnectedService).

## v1.3.2 `(2019-09-06)`

- [x] Update dependencies: add support for [NSwag.Commands v13.0.6](https://www.nuget.org/packages/NSwag.Commands/)

## v1.3.0 `(2019-06-29)`

- [x] Update dependencies: add support for [NSwag.Commands v13.0.3](https://www.nuget.org/packages/NSwag.Commands/)
- [x] Add `Inline named arays`, `Inline named dictonaries`, `Inline named tuples` and `Inline named any schemas` options to `CSharp Client Settings` and `CSharp Controller Settings` wizard pages
- [x] Add `Add model validation attributes` option to `CSharp Controller Settings` wizard page
- [x] Add `Any Type` textbox into `Primitive Types` group to `CSharp Client Settings` and `CSharp Controller Settings` wizard pages
- [x] Add `Inline named dictonaries` and `Inline named any schemas` options to `TypeScript Client Settings` wizard page

## v1.2.14 `(2019-06-23)`

- [x] Add support yaml-endpoints and endpoints not ends with ".json" or ".yml"

## v1.2.10 `(2019-05-22)`

- [x] Update dependencies: add support for [NSwag.Commands v12.3.1](https://www.nuget.org/packages/NSwag.Commands/)

## v1.2.9 `(2019-05-22)`

- [x] Update dependencies: add support for [NSwag.Commands v12.3.0](https://www.nuget.org/packages/NSwag.Commands/)

## v1.2.6 `(2019-05-01)`

- [x] Update dependencies: add support for [NSwag.Commands v12.2.4](https://www.nuget.org/packages/NSwag.Commands/)

## v1.2.4 `(2019-04-30)`

- [x] Add a button in the first wizard page for reporting bugs in github project [issues](https://github.com/unchase/Unchase.OpenAPI.Connectedservice/issues)

## v1.2.1 `(2019-04-22)`

- [x] Update dependencies: add support for [NSwag.Commands v12.2.0](https://www.nuget.org/packages/NSwag.Commands/)

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
