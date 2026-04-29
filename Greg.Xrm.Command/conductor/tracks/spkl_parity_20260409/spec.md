# Specification: spkl Parity (Developer Productivity)

## Overview
Migrate the two highest-value features from spkl that keep "pro-code" developers from switching to pac: attribute-based plugin registration and flexible web resource mapping.

## Scope
- **Attribute-Based Plugin Registration:** Scan compiled DLLs for `[CrmPluginStep]`, `[CrmPluginImage]`, and `[CrmWebhook]` attributes and auto-register plugin steps in Dataverse — effectively deprecating spkl for modern CI/CD.
- **Web Resource Watch & Map:** Map any local file to any web resource unique name (bypassing pac's rigid folder structure), with live sync on file change.
- **Plugin Step Validation:** Validate plugin step definitions without deploying to an environment.

## Constraints
- Must work with .NET 8 compiled assemblies.
- Must support incremental deployment (only changed plugins).
- Web resource mapping must handle legacy project structures with non-standard folder layouts.
- All commands must use the `IOutput` interface — no `Console.WriteLine`.

## Dependencies
- None — this is a foundational track that other tracks build upon.

## Success Criteria
- A developer can annotate a plugin class with `[CrmPluginStep]`, compile, and run `pacx plugin register-attributes` to register all steps.
- A developer can map arbitrary local files to web resources and run `pacx webresource watch` for live sync.
- Zero dependency on spkl for plugin registration workflows.

## API Readiness
- **Plugin Registration:** Dataverse Web API (pluginassembly, plugintype, sdkmessageprocessingstep) via `ServiceClient`
- **Web Resources:** Dataverse Web API (webresource) via `ServiceClient`
- **DLL Scanning:** `System.Reflection.Metadata` or `Mono.Cecil` for attribute extraction
