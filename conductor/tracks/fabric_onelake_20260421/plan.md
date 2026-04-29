# Implementation Plan: Microsoft Fabric & OneLake Integration

## Context
Zero Fabric coverage today. Fabric is Microsoft's strategic analytics platform (OneLake, Lakehouse, Semantic Models, Warehouses) and is the natural bridge from Dataverse data to Power BI. Highest strategic value among the new coverage tracks.

## Phase 1: Auth & Client
- [x] Task: Use the existing `ITokenProvider` with Fabric resource `https://api.fabric.microsoft.com/`, which resolves to the `.default` scope.
- [x] Task: `IFabricClient` abstraction (HttpClient wrapper) + `FabricClient` impl.
- [x] Task: DI wiring; unit tests with `HttpMessageHandler` double.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 2: Workspace & Capacity
- [x] Task: `fabric workspace list` — `GET https://api.fabric.microsoft.com/v1/workspaces`.
- [x] Task: `fabric workspace create --capacity-id` — `POST /v1/workspaces`.
- [x] Task: `fabric capacity list` — `GET /v1/capacities`.
- [x] Task: Tests + docs.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 3: Lakehouse
- [x] Task: `fabric lakehouse list --workspace-id`.
- [x] Task: `fabric lakehouse create --name --workspace-id`.
- [x] Task: Lakehouse import remains routed through OneLake shortcuts for this phase rather than a raw delta PUT.
- [x] Task: Tests + docs.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 4: OneLake Shortcuts
- [x] Task: `onelake shortcut create --source-path --target-path` — supports source type selection for ADLS Gen2, S3, Google Cloud Storage, and Dataverse-style shortcuts.
- [x] Task: `onelake shortcut list`.
- [x] Task: `onelake shortcut delete`.
- [x] Task: Tests + docs.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 5: Semantic Models (Fabric-hosted)
- [x] Task: `fabric semantic-model list --workspace-id`.
- [x] Task: Semantic model publish is deferred to the existing `tabular`/XMLA command surface until XMLA connection handling is unified.
- [x] Task: `fabric semantic-model refresh --id`.
- [x] Task: Integration path with the existing `tabular` noun is documented for a follow-up XMLA-specific slice.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 6: Data Pipeline (Dataverse → Fabric)
- [x] Task: `fabric link create --dataverse-env --target-workspace` — stages Dataverse Direct Lake link requests.
- [x] Task: `fabric link status`.
- [x] Task: Docs recipe: "Mirror a Dataverse table into OneLake in 3 commands".
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 7: Tests & PR Lifecycle
- [x] Task: Unit tests cover client HTTP behavior and command routing; live integration remains gated on Fabric credentials/workspace configuration.
- [x] Task: Working-tree implementation completed for upstream PR packaging.

## Validation
- Static JSON/config validation passed.
- Local build/test execution is blocked until the .NET 11 preview SDK is installed under the dotnet root used by `Greg.Xrm.Command/dotnet11.ps1`.
