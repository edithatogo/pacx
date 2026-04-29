# Specification: Code Quality Hardening

## Overview
Tighten the build quality and developer experience for the pacx (Greg.Xrm.Command) project by enforcing strict compilation rules, centralized dependency management, and a comprehensive static analysis stack.

## Scope
- **Central Package Management:** Use `Directory.Packages.props` for all NuGet versions.
- **Strict Compilation:** Enable `TreatWarningsAsErrors` across the solution.
- **Static Analysis Stack:** Integrate Meziantou, Roslynator, Threading, and BannedApi analyzers.
- **Supply Chain Security:** Enable NuGetAudit, lockfiles, and deterministic builds with SourceLink.
- **Architecture Enforcement:** Add architecture tests using `NetArchTest.Rules`.

## Success Criteria
- The solution builds successfully with `TreatWarningsAsErrors=true`.
- All NuGet versions are managed centrally; no `Version` attributes remain in `.csproj` files.
- The project is fully compliant with the new analyzer rules or has explicit justifications for any overrides.
- Deterministic builds are verified with embedded PDBs and SourceLink.
- Architecture rules (e.g., no cross-executor dependencies) are enforced by automated tests.

## Status
- **Phase 1: Central Package Management**: COMPLETED
- **Phase 2: Warnings-as-Errors**: PENDING
- **Phase 3: Analyzer Stack**: PENDING (Partially implemented in Directory.Packages.props)
- **Phase 4: Deterministic Builds**: PENDING (Partially implemented in Directory.Build.props)
- **Phase 5: Lock Files**: PENDING
- **Phase 6: Architecture Tests**: PENDING
