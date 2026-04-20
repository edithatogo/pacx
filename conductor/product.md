# Initial Concept
PACX (Greg.Xrm.Command) is a command-line tool for Dataverse that extends the capabilities of PAC CLI, aimed at Power Platform Developers to automate repetitive tasks and provide access to hidden features.

# Product Vision
PACX aims to be the standard open-source utility belt for Dataverse developers, providing a fast, efficient, and extensible platform for development and deployment. It follows a plugin-based architecture, allowing the community to build and share their own tools.

# Target Users
- Power Platform Developers
- Solution Architects
- DevOps Engineers working with Dataverse
- AI Agents (via MCP)

# Key Features
- Automation of repetitive Dataverse tasks.
- Access to platform features only available via API.
- Plugin-based extensibility for custom tools.
- **Model Context Protocol (MCP) Server:** Exposes CLI commands as tools for AI agents.
- **Advanced Automation Extensions:** Deep Flow management, run inspection, and connection visibility.
- **spkl Parity:** Attribute-based plugin registration and flexible web resource mapping.
- **Dataverse Platform Gaps:** CLI management for Custom APIs (Custom Actions), Catalog items, Elastic/Virtual tables, and Connection References.
- **Power BI Semantic Model CLI:** Tabular Editor 3 capabilities via CLI — BIM deploy, diff, validate, and translation management.
- Interactive mode for command discovery and execution.
- Command tree navigation and search.
- Colorized CLI output.

# Success Criteria
- Reduction in development time for Dataverse tasks.
- Broad adoption among Power Platform developers.
- A growing ecosystem of community-contributed plugins.
- Seamless integration with AI-assisted development workflows.

---

# Roadmap

## Quick Reference — Comma-Delimited Suggestion List
`data init-schema-from-solution, data seed-mock, webresource watch, webresource map, plugin register-attributes, plugin step-scan, plugin watch, plugin debug-session, admin user-onboard, admin settings-bulk-update, log stream-trace, solution diff, solution component-move, solution layer, connection-ref map-interactive, custom-api create, virtual-table scaffold, security-role clone, security audit-user, security sharing-report, catalog publish-item, elastic-table manage, quality gate, plugin trace-viewer, bulk-data count, bulk-data delete, workflow run list, workflow run get, workflow run resubmit, workflow run cancel, workflow get, workflow set-state, workflow trigger, connection list, connection validate, alm pipeline create, alm pipeline run, alm env-var sync, alm env diff, env create, env clone, env backup, env restore, env capacity report, env reset, pages site publish, pages webtemplate sync, pages site config export, pages site config import, pages liquid lint, ai model list, ai model train, ai model publish, ai form-processor configure, dlp policy audit, storage analytics, api ratelimit monitor, pcf test, pcf publish, pcf version bump, pcf dependency-check, connector import, connector export, connector test, connector validate, servicebus endpoint register, azure-function trigger configure, virtual-entity datasource manage, tabular deploy, tabular diff, tabular validate, tabular translate, tabular role-add-measures, tabular perspective-manage, bim compare, bim deploy, forms list, forms responses export, forms response count, pr open, pr track, pr merge, pr review-auto, e2e smoke-test, plugin test-scan`

---

## API Readiness Matrix

**Every proposed command maps to an existing, usable API.** No green-field server-side work is needed. The value is in CLI ergonomics, composition, and automation logic.

| Capability Area | Primary API | SDK / Library | Notes |
| :--- | :--- | :--- | :--- |
| **Plugin Registration** | Dataverse Web API (pluginassembly, plugintype, sdkmessageprocessingstep) | `Microsoft.PowerPlatform.Dataverse.ServiceClient` | CRUD on all plugin metadata tables. Well-documented. |
| **Web Resource Mapping** | Dataverse Web API (webresource) | `ServiceClient` | Upload, publish, map local files. Existing. |
| **Plugin Traces / Logs** | Dataverse Web API (plugintype, plugintrace, plugintracelog) + Application Insights API | `ServiceClient` + Azure SDK | Trace logs stored in Dataverse; App Insights for deeper telemetry. |
| **Flow / Power Automate** | Power Automate Management REST API (`api.bap.microsoft.com`) | `Microsoft.PowerPlatform.Cli` (internal) or direct HTTP | List runs, get details, resubmit, cancel, start/stop — all supported. |
| **Solution Management** | Dataverse Web API (solution, solutioncomponent) | `ServiceClient` | Query, compare, move components. Export/import via existing endpoints. |
| **Custom APIs** | Dataverse Web API (customapi, customapirequestparameter, customapiresponseproperty) | `ServiceClient` | Full CRUD — currently only GUI/solution manipulation exposes these. |
| **Security Roles & Privileges** | Dataverse Web API (role, systemuserroles, privilege, roleprivileges) + `RetrievePrincipalAccess` | `ServiceClient` | Role cloning, privilege auditing, user access analysis all feasible. |
| **Record Sharing** | Dataverse Web API (`RetrieveSharedPrincipalsAndAccess`, `GrantAccess`, `RevokeAccess`) | `ServiceClient` | PrincipalObjectAccess table for direct queries. |
| **Connections** | Dataverse Web API (connection, connectionreference) | `ServiceClient` | List, validate, map. Existing. |
| **Environment Management** | Power Platform Admin API (`api.bap.microsoft.com`) + Dataverse Management API | `Microsoft.PowerPlatform.Admin` | Create, clone, backup, reset, capacity — all available via BAP API. |
| **ALM / Pipelines** | Power Platform Admin API (pipelines) + Azure DevOps REST API / GitHub API | `ServiceClient` + Azure DevOps SDK | Pipeline creation, triggering, env var sync. |
| **Environment Variables** | Dataverse Web API (environmentvariabledefinition, environmentvariablevalue) | `ServiceClient` | Full CRUD across environments. |
| **Power Pages** | Dataverse Web API (adx_* tables — adx_website, adx_webtemplate, adx_page, adx_contentsnippet) | `ServiceClient` | All Power Pages config stored as Dataverse records. Publish via `adx_website` activation. |
| **AI Builder** | Power Platform Admin API + Dataverse Web API (aimodel, aibuilder_formprocessing) | `ServiceClient` | Model list, train, publish, configure. |
| **DLP Policies** | Power Platform Admin API (DlpPolicy) | `Microsoft.PowerPlatform.Admin` | List, audit, report on DLP policies. |
| **Storage Analytics** | Dataverse Web API (organization entity, storage API) + Power Platform Admin API | `ServiceClient` | Table-level storage via entity stats; file storage via environment API. |
| **PCF** | Dataverse Web API (solutioncomponent, webresource for PCF) | `ServiceClient` | Test via hosted test harness; publish via solution import. |
| **Custom Connectors** | Dataverse Web API (connector) + Power Platform Admin API | `ServiceClient` | Import/export as solution components; test via HTTP. |
| **Service Bus / Azure** | Azure Service Bus SDK + Dataverse Web API (serviceendpoint) | `Azure.Messaging.ServiceBus` | Register endpoints, configure triggers. |
| **Power BI (Tabular)** | Power BI REST API (`api.powerbi.com`) + XMLA Endpoint | `Microsoft.PowerBI.Api` | Deploy, diff, validate, translate — all available. Premium: XMLA; Pro: REST API. |
| **Catalog / Business Events** | Dataverse Web API (catalog, catalogitem) | `ServiceClient` | New feature; API exists but is less documented. |
| **Elastic Tables** | Dataverse Web API (table metadata with changefeed) | `ServiceClient` | Retention via table metadata; change feed configuration. |
| **Virtual Tables** | Dataverse Web API (externaldatasource, entitymap) | `ServiceClient` | Scaffold from data source metadata. |
| **API Rate Limits** | Dataverse Web API (organization settings) + HTTP response headers | `ServiceClient` | Throttle info in `x-ratelimit` headers; org settings for limits. |
| **Liquid / Power Pages Linting** | N/A (client-side processing) | Custom parser / Roslyn-style analyzer | Parse Liquid templates, check against known functions/objects. |
| **MS Forms** | Forms internal API (`forms.office.com/formapi/api/{tenantId}/{users|groups}/{ownerId}/`) | Direct HTTP + OAuth2 (Client Credentials for user forms, ROPC for group forms) | List forms, get responses (paged), response count, export to CSV/SQL. **Undocumented API** — group forms require ROPC (no MFA). Read-only for now; create/update deferred. |
| **GitHub PR Lifecycle** | GitHub REST API (`api.github.com`) | Octokit.NET | Open issues, create PRs, track review status, auto-apply conductor:review, detect merge conflicts, monitor PR acceptance. |

---

## Strategic Migration Questions

### 1. Migrate spkl Capabilities? — **Yes, High Priority**
The highest-value addition for "pro-code" developers is migrating the two features that keep them from switching from spkl to pac:

- **Attribute-Based Plugin Registration:** Scan compiled DLLs for `[CrmPluginStep]` attributes and auto-register plugin steps — effectively deprecating spkl for modern CI/CD.
- **Web Resource Mapping:** Map any local file to any web resource unique name, bypassing pac's rigid folder structure — crucial for legacy projects with messy structures.

### 2. Migrate XrmToolBox Features? — **Selectively**
Migrate "Admin Automation" tasks that are tedious in a GUI. Do **not** migrate complex visual editors.

**Migrate:**
- **Plugin Trace Viewer:** A `pacx log stream-trace` command — standard in every other modern cloud CLI (Azure, AWS).
- **User Settings Utility:** Bulk updating time zones/formats for many users is a scriptable task, not a GUI task.
- **Bulk Data Operations:** Simple "delete all records in Table X" or "count rows" commands.

**Do Not Migrate:**
- Visual editors (Ribbon Workbench, Sitemap Editor, etc.) — these are inherently GUI tasks.

### 3. CLI Mechanism for Plugin Management
**Yes.** No GUI needed. The Dataverse SDK (`ServiceClient`) already supports upserting plugin assemblies. The next step is a "definition" layer:

- **Attribute-based (spkl-style):** Scan compiled DLLs for `[CrmPluginStep]`, `[CrmPluginImage]`, and `[CrmWebhook]` attributes.
- **File-based (Terraform-style):** YAML/JSON definition files that the CLI reads to register Steps, Images, and Webhooks.

If Terraform can manage complex cloud infrastructure via text files, pacx can manage Plugin Steps via text files.

### 4. Mac/Linux Gap (.NET Core/6+ Solution)
The gap is not inherent — it exists because the official `pac data` commands wrap the legacy **Configuration Migration Tool (CMT)**, which relies on Windows Presentation Foundation (WPF) and full .NET Framework libraries.

**Roadmap Item:** Rewrite Data Export/Import using pure .NET 6+ `ServiceClient`.

**Feasibility:** High. The community has already done this in bits and pieces (e.g., Capgemini.Xrm.DataMigration). Integrating a pure .NET Core data engine into pacx would instantly solve the cross-platform data gap without waiting for Microsoft.

### 5. Existing pac Commands to Wrap/Fix in pacx
Rather than separate tools, pacx should wrap pac commands that are too granular or "dumb":

- **`pac solution check` → `pacx quality gate`:** The standard check creates a spreadsheet. pacx should parse that spreadsheet and return a strictly non-zero exit code if "High" severity issues are found (better for CI/CD).
- **`pac admin assign-user` → `pacx admin onboard`:** Wrap "Create User", "Assign Role", and "Add to Team" into a single transaction.

### 6. Tabular Editor 3 Capabilities in the PACX Pipeline — **Yes, High Priority**
Tabular Editor 3 is the de facto standard for Power BI semantic model (dataset) development, providing CI/CD-friendly editing of Tabular Object Model (TOM) via BIM (Tabular Model Scripting) files. Replicating its core capabilities into the pacx pipeline enables fully automated, scriptable semantic model management.

**Rationale:** Power BI semantic models are increasingly managed via CI/CD pipelines in enterprise environments. Tabular Editor 3 requires a desktop GUI. Bringing these capabilities to a CLI enables:
- Automated deployments from source control (Git) as part of CI/CD pipelines.
- Headless model comparison and validation between environments.
- Integration with Azure DevOps / GitHub Actions for Power BI deployment automation.
- Cross-platform support (Mac/Linux) without requiring a Windows desktop.

**Core Capabilities to Replicate:**
- **BIM File Deploy:** Deploy a `.bim` (Tabular Model Scripting) file to a Power BI dataset — the core deployment action Tabular Editor performs.
- **Model Diff:** Compare a local `.bim` file against a deployed Power BI semantic model and report differences — essential for CI/CD validation and drift detection.
- **Model Validation:** Validate a `.bim` file for common issues (circular dependencies, invalid references, best practices) before deployment.
- **Multi-Language Translation Management:** Manage and deploy translations for measures, columns, and tables — a key enterprise feature currently only available via Tabular Editor's GUI.
- **Role & Measure Bulk Operations:** Add measures to all roles, bulk-create role definitions, manage perspectives — repetitive admin tasks that benefit from CLI automation.

**Technical Approach:**
- Use the Power BI REST API (`Microsoft.PowerBI.Api`) for dataset manipulation.
- Parse and manipulate `.bim` files (JSON-based TOM serialization) using `Microsoft.AnalysisServices.Core` or the open-source TOM wrapper libraries.
- Implement idempotent deploy operations that only push changes (not full model overwrites).
- Support both XMLA endpoint (Premium/Embedded) and REST API (Pro) connectivity modes.

### 7. Microsoft Forms API — **Yes, Medium Priority**
The Forms API at `forms.office.com/formapi/api/` is undocumented but fully functional. It enables reading forms, exporting responses, and monitoring response counts — all without GUI interaction.

**Rationale:** Organizations with thousands of Forms need programmatic access for compliance, reporting, and data archival. Currently, the only options are manual Excel export or the maker portal.

**Core Capabilities:**
- `forms list` — List all forms with metadata (ID, title, status, response count, owner).
- `forms responses export` — Export responses to CSV, JSON, or SQL (paged, with `$skip` for incremental sync).
- `forms response count` — Quick count of responses for monitoring/alerting.

**Limitations:**
- **Undocumented API** — not part of Microsoft Graph; could change without notice.
- **Group forms** require ROPC flow (no MFA) — requires a dedicated service account.
- **User forms** work with Application permissions (Client Credentials) — cleaner for automation.
- Create/update/delete not yet available — read-only for now.

**Technical Approach:**
- Direct HTTP calls with OAuth2 token management (auto-refresh, 401 retry).
- `Microsoft.Identity.Client` (MSAL) for token acquisition.
- Paged response handling with `$skip`/`$top` for large datasets.

---

## Repository & Engineering Improvements

### Current State Assessment

| Area | Current State | Recommendation |
| :--- | :--- | :--- |
| **CI runs tests** | **No** — `dotnet test` not in pipeline | **Critical** — add test execution immediately |
| **Code coverage** | coverlet referenced, never invoked | Configure with `.runsettings`, enforce 80%+ |
| **`.editorconfig`** | Absent | Create — enforces C# style automatically |
| **`.runsettings`** | Absent | Create — configures coverage collection |
| **`Directory.Build.props`** | Absent | Create — centralize common MSBuild properties |
| **Static analysis** | `EnforceCodeStyleInBuild` set but no `.editorconfig` | Add SonarQube or CodeQL |
| **Mutation testing** | None | Add Stryker.NET for test quality |
| **Property-based testing** | None | Add FsCheck for edge-case discovery |
| **Integration tests** | None — all mocked | Add against real test environment |
| **E2E/smoke tests** | None | Add post-deploy smoke test in CI |
| **Typing/validation** | C# (strongly typed) | Add nullable reference types, analyzers |
| **Profiling** | None | Add BenchmarkDotNet for hot paths |

### Recommended CI/CD Pipeline (SOTA)

**Phase 1: Immediate (Must-Have)**
1. **Add `dotnet test` to CI** — non-negotiable baseline
2. **Add `.editorconfig`** — Google C# style guide compliance
3. **Add `.runsettings`** — cobertura coverage collection
4. **Add coverage threshold** — fail CI if <80%
5. **Add `dotnet format --verify-no-changes`** — style enforcement
6. **Upload coverage to Codecov or Coveralls** — trend tracking

**Phase 2: Quality Gates (Should-Have)**
7. **GitHub CodeQL analysis** — security vulnerability scanning
8. **Stryker.NET mutation testing** — measures test effectiveness (not just coverage)
9. **Nullable reference types** — enable `<Nullable>enable</Nullable>` across all projects
10. **Roslyn analyzers** — `Microsoft.CodeAnalysis.NetAnalyzers` + `SonarAnalyzer.CSharp`

**Phase 3: Advanced (Nice-to-Have)**
11. **FsCheck property-based testing** — discover edge cases in parsing/validation logic
12. **BenchmarkDotNet** — profile command execution time, memory allocation
13. **E2E smoke tests** — run against a test Dataverse environment after every PR merge
14. **Dependabot/Renovate** — automated dependency updates
15. **PR size labels** — auto-label PRs by lines changed for review routing

**Phase 4: Engineering Infrastructure (Recommended)**
16. **`Directory.Build.props`** — Centralize MSBuild properties across all 6 projects (eliminate duplication of TargetFramework, Nullable, package versions, NuGet metadata).
17. **`track-schema.json`** — JSON Schema for `metadata.json` validation. Enables querying ("show all High priority tracks"), validation (catch missing fields), and tooling (auto-generate status dashboards).
18. **E2E smoke test infrastructure** — Dedicated integration test project + CI workflow against real Dataverse environment.
19. **Plugin loading test coverage** — Currently 0%. Add tests for CommandRegistry, PluginLoader, Bootstrapper with mock plugin DLLs. Target: >90% coverage.

### PR Lifecycle Automation Protocol

**Problem:** Currently, after each feature track is completed, opening an issue, creating a PR, and monitoring for merge conflicts is manual. PRs can sit unreviewed indefinitely. Review-fix cycles require multiple manual passes.

**Solution:** Add a standardized **PR Lifecycle (Ralph Loop) Phase** to every track's plan. The Ralph loop self-drives the review-fix cycle until the PR is actually ready.

**Standard Phase Template (added to every track):**
```markdown
## Phase N: PR Lifecycle (Ralph Loop)
- [ ] Task: Open a GitHub issue describing the feature/fix.
- [ ] Task: Create a PR against the upstream repo with implementation.
- [ ] Task: Run `/ralph-loop` on the PR with completion promise:
          "All Critical and High review issues resolved, PR ready for merge"
- [ ] Task: Confirm PR is merged or document blockers.
```

**How the Ralph Loop Works:**
1. Enters the loop with the PR URL and completion promise.
2. Runs `/conductor:review` on the PR.
3. Applies all suggested fixes automatically.
4. Commits the fixes.
5. Re-runs `/conductor:review`.
6. Checks the completion promise — are there still Critical/High issues? If yes → repeat. If no → promise fires, loop exits.
7. Reports the final state: merged, or blockers documented.

**Why Ralph Loop Over Manual Steps:**
| Manual Approach | Ralph Loop |
|----------------|-----------|
| Assumes one review pass is enough | Iterates until the PR actually passes |
| Reviewer fatigue — easy to forget re-review | Automatic enforcement |
| Arbitrary "max 2 fix attempts" limit | Continues until the promise is genuinely true |
| Manual monitoring for conflicts | Self-driving until completion |

---

## Phased Roadmap

### Phase 1: Developer Productivity (spkl Parity)
| Command | Description | Priority |
| :--- | :--- | :--- |
| `plugin register-attributes` | Scan DLLs for `[CrmPluginStep]` attributes and auto-register plugin steps. | **High** |
| `plugin step-scan` | Validate plugin step definitions without deploying. | **High** |
| `webresource watch` | Map local files to web resources, with live sync on change. | **High** |
| `webresource map` | Flexible file-to-resource mapping (spkl-style). | **High** |

### Phase 2: Admin Automation (XrmToolBox → CLI)
| Command | Description | Priority |
| :--- | :--- | :--- |
| `log stream-trace` | Tail plugin trace logs in real-time. | **High** |
| `admin user-onboard` | Create user, assign roles, add to teams in one transaction. | **Medium** |
| `admin settings-bulk-update` | Bulk-update user settings (timezone, format, language). | **Medium** |
| `bulk-data count` | Count rows in a table. | **Medium** |
| `bulk-data delete` | Delete all records in a table (with confirmation/safety). | **Medium** |
| `security-role clone` | Clone a security role with all privileges. | **Low** |

### Phase 3: Dataverse Platform Gaps
| Command | Description | Priority |
| :--- | :--- | :--- |
| `custom-api create` | Create Custom APIs (Custom Actions) via CLI — currently GUI-only or messy solution manipulation. | **High** |
| `catalog publish-item` | Manage Catalog & Business Events for exposing Dataverse events externally. | **Medium** |
| `elastic-table manage` | Manage retention policies and scaling for Elastic Tables. | **Medium** |
| `virtual-table scaffold` | Scaffold virtual table definitions from data sources. | **Low** |
| `connection-ref map-interactive` | Interactive mapping of connection references across solutions. | **Low** |

### Phase 4: CI/CD Quality & Solution Management
| Command | Description | Priority |
| :--- | :--- | :--- |
| `quality gate` | Parse `pac solution check` results; return non-zero exit code on "High" severity issues. | **High** |
| `solution diff` | Compare two solutions or environments and report component differences. | **High** |
| `solution component-move` | Move individual components between solutions (not whole-solution import/export). | **Medium** |

### Phase 5: Data & Cross-Platform
| Command | Description | Priority |
| :--- | :--- | :--- |
| `data init-schema-from-solution` | Generate schema definition from an existing solution. | **Medium** |
| `data seed-mock` | Generate mock/seed data for development environments. | **Medium** |
| **Pure .NET 6+ Data Engine** | Rewrite data export/import to eliminate WPF/CMT dependency — enables Mac/Linux support. | **High** |

### Phase 6: Power BI Semantic Model (Tabular Editor CLI)
| Command | Description | Priority |
| :--- | :--- | :--- |
| `tabular deploy` | Deploy a `.bim` file to a Power BI semantic model (idempotent). | **High** |
| `tabular diff` | Compare local `.bim` against deployed model — report drift. | **High** |
| `tabular validate` | Validate `.bim` for circular deps, invalid refs, best practices. | **High** |
| `tabular translate` | Manage and deploy multi-language translations for measures/columns. | **Medium** |
| `tabular role-add-measures` | Bulk-add measures to all security roles. | **Medium** |
| `tabular perspective-manage` | Create, update, and manage model perspectives via CLI. | **Low** |
| `bim compare` | Compare two `.bim` files and output structural differences. | **Medium** |
| `bim deploy` | Deploy `.bim` via XMLA endpoint (Premium/Embedded workspaces). | **High** |

### Phase 7: ALM Center Automation
| Command | Description | Priority | API |
| :--- | :--- | :--- | :--- |
| `alm pipeline create` | Create a deployment pipeline stage from template. | **High** | Power Platform Admin API |
| `alm pipeline run` | Trigger a pipeline stage (Validate → Deploy → Configure). | **High** | Power Platform Admin API |
| `alm env-var sync` | Sync environment variables across environments with value mapping. | **High** | Dataverse Web API |
| `alm env diff` | Compare two environments: tables, columns, solutions, env vars, connections. | **High** | Dataverse Web API + Admin API |
| `solution layer` | Manage solution layers — version pinning, dependency resolution. | **Medium** | Dataverse Web API |

### Phase 8: Power Pages CLI
| Command | Description | Priority | API |
| :--- | :--- | :--- | :--- |
| `pages site publish` | Publish a Power Pages site from local source. | **High** | Dataverse Web API (adx_website) |
| `pages webtemplate sync` | Sync web templates, page templates, content snippets between environments. | **High** | Dataverse Web API (adx_webtemplate, adx_contentsnippet) |
| `pages site config export` | Export portal configuration (auth, navigation, themes). | **Medium** | Dataverse Web API |
| `pages site config import` | Import portal configuration with conflict resolution. | **Medium** | Dataverse Web API |
| `pages liquid lint` | Validate Liquid templates for errors before deployment. | **Medium** | Client-side analyzer |

### Phase 9: Environment Lifecycle Management
| Command | Description | Priority | API |
| :--- | :--- | :--- | :--- |
| `env create` | Create environment (Developer, Sandbox, Production) with settings. | **High** | Power Platform Admin API (BAP) |
| `env clone` | Clone environment — schema only, schema + data, or selective tables. | **High** | BAP API + Dataverse API |
| `env backup` | Trigger database backup and monitor progress. | **Medium** | BAP API |
| `env restore` | Restore from a specific backup point. | **Medium** | BAP API |
| `env capacity report` | Report database/file storage capacity across all environments. | **Medium** | BAP Admin API |
| `env reset` | Reset sandbox to factory state. | **Low** | BAP API |

### Phase 10: Governance, Security & Monitoring
| Command | Description | Priority | API |
| :--- | :--- | :--- | :--- |
| `security audit-user` | Full privilege audit: what can a user actually do? (field, record, hierarchy) | **High** | Dataverse Web API |
| `security sharing-report` | Who has access to a record, and why? (share, team, BU, role) | **High** | PrincipalObjectAccess table |
| `dlp policy audit` | Review and report DLP policy coverage across connectors/environments. | **Medium** | Power Platform Admin API |
| `storage analytics` | Table-by-table storage analysis with cleanup recommendations. | **Medium** | Dataverse Web API + Admin API |
| `api ratelimit monitor` | Monitor and alert on API rate limit proximity. | **Low** | HTTP response headers |

### Phase 11: PCF Enhancement
| Command | Description | Priority | API |
| :--- | :--- | :--- | :--- |
| `pcf test` | Run PCF component tests in headless mode for CI/CD. | **High** | PCF test harness |
| `pcf publish` | Publish a PCF component without full solution import. | **High** | Dataverse Web API |
| `pcf version bump` | Semantic version management for PCF components. | **Medium** | Local file + manifest |
| `pcf dependency-check` | Validate PCF dependencies are satisfied in target environment. | **Medium** | Dataverse Web API |

### Phase 12: AI Builder & Custom Connectors
| Command | Description | Priority | API |
| :--- | :--- | :--- | :--- |
| `ai model list` | List AI Builder models with training status and accuracy. | **Medium** | Dataverse Web API (aimodel) |
| `ai model train` | Trigger model training from labeled data. | **Medium** | AI Builder API |
| `ai model publish` | Publish a trained model to an environment. | **Medium** | AI Builder API |
| `ai form-processor configure` | Configure form processing models (document type, fields, tables). | **Low** | AI Builder API |
| `connector import` | Import a custom connector from definition file. | **Medium** | Dataverse Web API (connector) |
| `connector export` | Export a custom connector to a definition file. | **Medium** | Dataverse Web API |
| `connector test` | Test custom connector operations with sample payloads. | **Low** | HTTP + connector definition |
| `connector validate` | Validate connector definition against OpenAPI schema. | **Low** | Schema validation |

---

## Appendix: Cross-Cutting Capabilities

### Azure Integration Layer
| Command | Description | Priority | API |
| :--- | :--- | :--- | :--- |
| `servicebus endpoint register` | Register Azure Service Bus endpoint for Dataverse service endpoints. | **Medium** | Dataverse Web API (serviceendpoint) |
| `azure-function trigger configure` | Configure Azure Function triggers for Dataverse events. | **Medium** | Dataverse Web API + Azure Function Admin API |
| `virtual-entity datasource manage` | Create and manage virtual entity data sources. | **Low** | Dataverse Web API (externaldatasource) |

### Developer Experience (Hot Reload)
| Command | Description | Priority | API |
| :--- | :--- | :--- | :--- |
| `plugin watch` | Watch compiled DLL for changes and hot-update the assembly in Dataverse. | **High** | Dataverse Web API (pluginassembly upsert) |
| `plugin debug-session` | Start a debug session with Profiler integration. | **Medium** | Plugin Profiler API |
