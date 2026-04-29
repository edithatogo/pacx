# Specification: Plugin Loading Test Coverage

## Overview
The plugin loading mechanism (`CommandRegistry.cs`, `McMaster.NETCore.Plugins` integration, `Bootstrapper`) has zero test coverage. A failing plugin load breaks every downstream command and affects every user who installs plugins. This track adds comprehensive unit and integration tests for the plugin system.

## Scope
- **CommandRegistry Tests:** Test command scanning, duplicate verb detection, command tree building, and namespace helper discovery.
- **PluginLoader Tests:** Test assembly loading from disk, `PreferSharedTypes` behavior, plugin folder scanning, `.delete` marker handling, and ad-hoc `--tool` loading.
- **Bootstrapper Tests:** Test the full initialization flow: registry init → core assembly scan → plugin scan → command runner creation.
- **Mock Plugin Tests:** Create test plugins (as separate DLLs) that exercise edge cases: empty plugin, plugin with duplicate verbs, plugin with obsolete commands, plugin with multiple `IModule` implementations.

## Constraints
- Tests must not require a real Dataverse connection — the plugin system is purely local file/DLL operations.
- Mock plugin DLLs must be built as part of the test project or loaded from pre-built test assemblies.
- Tests must be deterministic — no file system race conditions or timing-dependent behavior.

## Dependencies
- None — tests the existing plugin loading code in `Greg.Xrm.Command.Core`.

## Success Criteria
- Plugin loading code has >90% code coverage (currently 0%).
- All edge cases tested: empty plugin folder, corrupt DLL, duplicate verbs, missing executor, obsolete commands.
- Tests run in under 30 seconds total.
- No tests depend on external state (fully isolated).

## API Readiness
- **Plugin Loading:** `McMaster.NETCore.Plugins` (local DLL loading — no network API)
- **Command Scanning:** `System.Reflection` / `System.Reflection.MetadataLoadContext` (local DLL introspection)
- **Autofac Module Scanning:** `Autofac.Core.IModule` interface scanning via reflection
