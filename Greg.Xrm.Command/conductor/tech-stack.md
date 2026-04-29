# Tech Stack

## Programming Language
- C# (.NET 8)

## Target Platform
- Microsoft Power Platform / Dataverse

## Project Type
- Dotnet Global Tool (CLI)

## Architecture
- Modular, Plugin-based Architecture with Dependency Injection.
- Core logic separated into `Greg.Xrm.Command.Core`.
- Interfaces defined in `Greg.Xrm.Command.Interfaces`.
- Plugins loaded dynamically via `McMaster.NETCore.Plugins`.
- Hybrid DI: `Microsoft.Extensions.DependencyInjection` + `Autofac` (using `Module` registrations).
- **Model Context Protocol:** Integrated via `ModelContextProtocol` SDK.

## Quality Infrastructure
- Root `.editorconfig` for consistent formatting.
- Unit tests using MSTest and Moq in `Greg.Xrm.Command.Core.TestSuite`.
- Automated code coverage using `coverlet.collector`.
