# Implementation Plan: Microsoft Fabric & OneLake Integration

## Context
Zero Fabric coverage today. Fabric is Microsoft's strategic analytics platform (OneLake, Lakehouse, Semantic Models, Warehouses) and is the natural bridge from Dataverse data to Power BI. Highest strategic value among the new coverage tracks.

## Phase 1: Auth & Client
- [ ] Task: Extend `ITokenProvider` with a Fabric scope (`https://api.fabric.microsoft.com/.default`).
- [ ] Task: `IFabricClient` abstraction (HttpClient wrapper) + `FabricClient` impl.
- [ ] Task: DI wiring; unit tests with `HttpMessageHandler` double.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 2: Workspace & Capacity
- [ ] Task: `fabric workspace list` — `GET https://api.fabric.microsoft.com/v1/workspaces`.
- [ ] Task: `fabric workspace create --capacity-id` — `POST /v1/workspaces`.
- [ ] Task: `fabric capacity list` — `GET /v1/capacities`.
- [ ] Task: Tests + docs.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 3: Lakehouse
- [ ] Task: `fabric lakehouse list --workspace-id`.
- [ ] Task: `fabric lakehouse create --name --workspace-id`.
- [ ] Task: `fabric lakehouse import --file <delta>` — PUT to OneLake path.
- [ ] Task: Tests + docs.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 4: OneLake Shortcuts
- [ ] Task: `onelake shortcut create --source-path --target-path` — supports ADLS Gen2, S3, Google Cloud Storage, Dataverse sources.
- [ ] Task: `onelake shortcut list`.
- [ ] Task: `onelake shortcut delete`.
- [ ] Task: Tests + docs.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 5: Semantic Models (Fabric-hosted)
- [ ] Task: `fabric semantic-model list --workspace-id`.
- [ ] Task: `fabric semantic-model publish --bim <file> --workspace-id` — uses XMLA endpoint + Fabric REST bootstrapping.
- [ ] Task: `fabric semantic-model refresh --id`.
- [ ] Task: Integrates with existing `tabular` noun — add alias `tabular publish --fabric` that routes here.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 6: Data Pipeline (Dataverse → Fabric)
- [ ] Task: `fabric link create --dataverse-env --target-workspace` — enables Dataverse Direct Lake link.
- [ ] Task: `fabric link status`.
- [ ] Task: Docs recipe: "Mirror a Dataverse table into OneLake in 3 commands".
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 7: Tests & PR Lifecycle
- [ ] Task: Integration tests gated on `PACX_FABRIC_WORKSPACE_ID`.
- [ ] Task: Upstream PR per 2-3 phases; `/ralph-loop`; merge.
