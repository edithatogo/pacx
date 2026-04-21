# Audit: Stubbed Executors

This file lists all executors that contain `[DRY RUN]` or placeholder text instead of real API calls.

| File | Line | Command / Action | Priority | Strategy |
|------|------|------------------|----------|----------|
| `AlmCommandExecutors.cs` | 13 | `alm pipeline create` | High | Done (BAP API) |
| `AlmCommandExecutors.cs` | 47 | `alm pipeline run` | High | Done (BAP API) |
| `AiBuilderCommandExecutors.cs` | 112 | `aibuilder publish` | Medium | Done (Real API via AiBuilderApiClient) |
| `CatalogPublishCommandExecutor.cs` | 25 | `catalog publish` | High | Real (Dataverse) |
| `ConnectorCommandExecutors.cs` | 25 | `connector import` | High | Done (Real Dataverse API) |
| `DataExportImportCommandExecutors.cs` | 151 | `data import` | Medium | Real (Dataverse) |
| `PagesCommandExecutors.cs` | 31 | `pages publish` | Low | Real (Dataverse) |
| `PagesCommandExecutors.cs` | 59 | `pages template sync` | Low | Partial (Dataverse) |
| `PagesSiteConfigCommandExecutors.cs` | 165 | `pages site-config import` | Low | Real (Dataverse) |
| `PcfCommandExecutors.cs` | 61 | `pcf publish` | Low | Real (Dataverse) |
| `PrMergeCommandExecutor.cs` | 52 | `pr merge` | Medium | Real (GitHub) |
| `PrOpenCommandExecutor.cs` | 37 | `pr open` | Medium | Real (GitHub) |
| `SolutionComponentMoveCommandExecutor.cs` | 49 | `solution component-move` | Medium | Real (Dataverse) |
| `TabularAdvancedCommandExecutors.cs` | 30 | `tabular translation export` | Low | Real (BIM) |
| `TabularAdvancedCommandExecutors.cs` | 35 | `tabular translation compare` | Low | Real (BIM) |
| `TabularCommandExecutors.cs` | 34 | `tabular deploy` | Medium | Real (XMLA) |
| `VirtualTableScaffoldCommandExecutor.cs` | 29 | `virtualtable scaffold` | Medium | Real (Dataverse) |
| `WebResourceMapCommandExecutor.cs` | 62 | `webresource map` | High | Real (Local/Dataverse) |

## Notes
- `[DRY RUN]` in `PluginRegisterAttributesCommandExecutor.cs` is intentional (handling `--dry-run` flag).
- ALM and Connectors are the most critical "fake" implementations identified by reviewers.
