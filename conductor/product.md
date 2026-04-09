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
`data init-schema-from-solution, data seed-mock, webresource watch, plugin register-attributes, plugin step-scan, admin user-onboard, admin settings-bulk-update, log stream-trace, solution diff, solution component-move, connection-ref map-interactive, custom-api create, virtual-table scaffold, security-role clone, catalog publish-item, elastic-table manage, quality gate, plugin trace-viewer, bulk-data count, bulk-data delete, workflow run list, workflow run get, workflow run resubmit, workflow run cancel, workflow get, workflow set-state, connection list, tabular deploy, tabular diff, tabular validate, tabular translate, tabular role-add-measures, tabular perspective-manage, bim compare, bim deploy`

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
