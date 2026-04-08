# PAC & PACX CLI - Complete Tool Inventory

## ✅ PAC CLI (Power Platform CLI)

**Version:** 2.5.1+gab954cf  
**Location:** `C:\Users\60217257\.dotnet\tools\pac.exe`  
**Authentication:** ✅ Active (Dylan.Mordaunt@health.nsw.gov.au)  
**Environment:** Illawarra Shoalhaven LHD (https://orgefc9aa3e.crm6.dynamics.com/)

### All PAC Modules/Commands:

| Module | Status | Description |
|--------|--------|-------------|
| `pac admin` | ✅ Available | Power Platform Admin Account management |
| `pac application` | ✅ Available | List/install Dataverse applications from Marketplace |
| `pac auth` | ✅ Active | Authentication management |
| `pac canvas` | ✅ Available | Power Apps .msapp file operations |
| `pac catalog` | ✅ Available | Power Platform Catalog operations |
| `pac code` | ✅ Available (Preview) | Manage Code apps |
| `pac connection` | ✅ Available | Dataverse connection management |
| `pac connector` | ✅ Available | Power Platform Connectors |
| `pac copilot` | ✅ Available | Copilot tools and utilities |
| `pac copilot mcp` | ✅ Available | Run local MCP server (`pac copilot mcp --run`) |
| `pac data` | ✅ Available | Import/export Dataverse data |
| `pac env` | ✅ Available | Dataverse organization operations |
| `pac managed-identity` | ✅ Available | Managed Identity records for Dataverse components |
| `pac model` | ✅ Available (Preview) | Model-driven apps |
| `pac modelbuilder` | ✅ Available | Code Generator for Dataverse APIs and Tables |
| `pac package` | ✅ Available | Dataverse package projects |
| `pac pages` | ✅ Available | Power Pages website operations |
| `pac pcf` | ✅ Available | Power Apps Component Framework projects |
| `pac pipeline` | ✅ Available | Pipeline operations |
| `pac plugin` | ✅ Available | Dataverse plug-in class library |
| `pac power-fx` | ✅ Available (Preview) | Power Fx operations |
| `pac solution` | ✅ Available | Dataverse solution projects |
| `pac telemetry` | ✅ Available | Telemetry settings |
| `pac test` | ✅ Available (Preview) | Automated tests for Power Apps |
| `pac tool` | ✅ Available | Power Platform tools management |

### PAC Tools Installed:

| Tool | Version | Status | Description |
|------|---------|--------|-------------|
| **CMT** | 9.1.0.185 | ✅ Installed | Configuration Migration Tool |
| **PD** | 9.1.0.182 | ✅ Installed | Package Deployer |
| **PRT** | 9.1.0.200 | ✅ Installed | Plugin Registration Tool |

---

## ✅ PACX CLI (Greg.Xrm.Command)

**Version:** 1.2026.3.195  
**Location:** `C:\Users\60217257\.dotnet\tools\pacx.exe`  
**Runtime:** .NET 8.0.25 (x64)  
**Authentication:** ✅ **Authenticated**  
**Profile:** NSW Health  
**Environment:** https://orgefc9aa3e.crm6.dynamics.com/

### All PACX Modules/Commands:

| Module | Status | Description |
|--------|--------|-------------|
| `pacx auth` | ✅ Available | Manage authentication to Dataverse environments |
| `pacx column` | ✅ Available | Dataverse column manipulations |
| `pacx forms` | ✅ Available | Form manipulation commands |
| `pacx history` | ✅ Available | Command history access |
| `pacx key` | ✅ Available | Dataverse key manipulations |
| `pacx optionset` | ✅ Available | Global/local option sets (Picklists) |
| `pacx org` | ✅ Available | Organization settings |
| `pacx plugin` | ✅ Available | Plugin registration & step management |
| `pacx project` | ✅ Available | PACX project lifecycle |
| `pacx publish` | ✅ Available | Manual customization publishing |
| `pacx rel` | ✅ Available | Dataverse relationship operations |
| `pacx ribbon` | ✅ Available | Ribbon (command bar) manipulation |
| `pacx script` | ✅ Available | Generate PACX scripts and OptionSet CSV |
| `pacx settings` | ✅ Available | Dataverse settings manipulation |
| `pacx solution` | ✅ Available | Dataverse solution management |
| `pacx table` | ✅ Available | Dataverse table manipulations |
| `pacx tool` | ✅ Available | PACX tool/extension management |
| `pacx unifiedrouting` | ✅ Available | Unified routing settings |
| `pacx view` | ✅ Available | Dataverse view manipulations |
| `pacx webresources` | ✅ Available | Web resource operations |
| `pacx workflow` | ✅ Available | Power Automate flows / legacy workflows |

### PACX Tools/Extensions:

| Tool | Status | Description |
|------|--------|-------------|
| *(None installed)* | ⚪ Empty | Run `pacx tool install <tool>` to add extensions |

**Note:** PACX extensions are community-contributed. No official extension packages exist on NuGet at this time. You can create custom tools as DLLs.

---

## 📋 Feature Comparison: PAC vs PACX

| Feature Area | PAC CLI | PACX CLI | Notes |
|--------------|---------|----------|-------|
| **Authentication** | ✅ `pac auth` | ✅ `pacx auth` | PAC uses Azure AD, PACX uses Dataverse auth |
| **Solution Management** | ✅ `pac solution` | ✅ `pacx solution` | PAC: import/export, PACX: more granular control |
| **Plugin Registration** | ✅ `pac plugin` | ✅ `pacx plugin` | PAC: basic, PACX: full PluginRegistrationTool |
| **PCF Development** | ✅ `pac pcf` | ❌ | PAC only |
| **Canvas Apps** | ✅ `pac canvas` | ❌ | PAC only |
| **Power Pages** | ✅ `pac pages` | ❌ | PAC only |
| **Dataverse Tables** | ❌ | ✅ `pacx table` | PACX only |
| **Dataverse Columns** | ❌ | ✅ `pacx column` | PACX only |
| **Dataverse Views** | ❌ | ✅ `pacx view` | PACX only |
| **Dataverse Relationships** | ❌ | ✅ `pacx rel` | PACX only |
| **Forms** | ❌ | ✅ `pacx forms` | PACX only |
| **Ribbons** | ❌ | ✅ `pacx ribbon` | PACX only |
| **Web Resources** | ❌ | ✅ `pacx webresources` | PACX only |
| **Workflows/Flows** | ❌ | ✅ `pacx workflow` | PACX only (list/activate/deactivate) |
| **Option Sets** | ❌ | ✅ `pacx optionset` | PACX only |
| **Settings** | ❌ | ✅ `pacx settings` | PACX only |
| **Unified Routing** | ❌ | ✅ `pacx unifiedrouting` | PACX only |
| **MCP Server** | ✅ `pac copilot mcp` | ❌ | PAC only |
| **Model-driven Apps** | ✅ `pac model` | ❌ | PAC only |
| **Data Operations** | ✅ `pac data` | ❌ | PAC only |
| **Environment Mgmt** | ✅ `pac env` | ❌ | PAC only |
| **Pipeline** | ✅ `pac pipeline` | ❌ | PAC only |
| **Code Generator** | ✅ `pac modelbuilder` | ❌ | PAC only |
| **Managed Identity** | ✅ `pac managed-identity` | ❌ | PAC only |
| **Telemetry** | ✅ `pac telemetry` | ❌ | PAC only |
| **Testing** | ✅ `pac test` | ❌ | PAC only |

---

## 🔐 Authentication Status

| CLI | Status | Command to Setup |
|-----|--------|------------------|
| **PAC** | ✅ Authenticated | Already active |
| **PACX** | ✅ **Authenticated** | Profile: NSW Health |

### Setup PACX Authentication:
```bash
pacx auth
```

This will prompt you to:
1. Enter your Dataverse environment URL
2. Authenticate with Microsoft account
3. Save the profile for future use

---

## 🚀 Quick Start Commands

### PAC CLI:
```bash
# Check authentication
pac auth list

# List environments
pac env list

# List solutions
pac solution list

# List available tools
pac tool list

# Start MCP server (for AI integration)
pac copilot mcp --run
```

### PACX CLI:
```bash
# Setup authentication
pacx auth

# List tables
pacx table list

# List solutions
pacx solution list

# List workflows
pacx workflow list

# Interactive mode
pacx --interactive
```

---

## 📦 Additional Tools (XrmToolBox)

These are GUI tools (not CLI) by the same author (neronotte):

| Tool | Description | Location |
|------|-------------|----------|
| **EnvironmentComparer** | Compare solutions across environments | XrmToolBox plugin |
| **SolutionManager** | Monitor solution import progress | XrmToolBox plugin |
| **DataModelWikiEditor** | Generate markdown docs for Dataverse entities | XrmToolBox plugin |
| **RoleEditor** | Edit security roles in Dataverse | XrmToolBox plugin |

**Note:** These are separate from PACX and require XrmToolBox installation.

---

## 🔧 Maintenance

### Update PAC CLI:
```bash
pac install latest
```

### Update PACX CLI:
```bash
dotnet tool update -g Greg.Xrm.Command
```

### Add PACX Extension:
```bash
pacx tool install <tool-name-or-path>
```

### List PAC Tools:
```bash
pac tool list
```

### List PACX Tools:
```bash
pacx tool list
```

---

## ✅ Summary

**PAC CLI:** ✅ All modules and tools installed and ready  
**PACX CLI:** ✅ All modules installed, authentication pending  
**Extensions:** ⚪ No community extensions available (PACX tool system is open for custom development)

**Total PAC Commands:** 23 modules + 3 GUI tools  
**Total PACX Commands:** 21 modules + extension framework

---

**Last Updated:** 8 April 2026  
**System:** Windows 11 (win32)  
**.NET Runtime:** 8.0.25 (x64), 10.0.201 (SDK)
