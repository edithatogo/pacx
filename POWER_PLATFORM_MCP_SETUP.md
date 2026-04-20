# Power BI & Power Platform MCP Setup Guide

This document describes the MCP servers and tools configured for Power BI and Power Platform integration.

## ✅ Installed & Ready

### 1. Power BI Modeling MCP (`powerbi-modeling`)
**Status:** ✅ Installed locally, not configured in this repo

**What it does:** Enables AI-powered Power BI semantic model manipulation and DAX query generation.

**Location:** `npx @microsoft/powerbi-modeling-mcp@latest --start`

**Authentication:** 
- Default: Interactive Azure AD login (will prompt when first used)
- Service Principal: Set environment variables:
  - `AZURE_CLIENT_ID`
  - `AZURE_TENANT_ID`
  - `AZURE_CLIENT_SECRET` or `AZURE_CLIENT_CERTIFICATE_PATH`

**Usage:**
- Automatically starts when MCP client connects
- Respects Power BI/Fabric RBAC permissions
- Works with your existing Power BI access

---

### 2. Greg.Xrm.Command (PACX)
**Status:** ✅ Installed (CLI only)

**What it does:** Enhanced CLI for Dataverse and Power Platform with extensive schema management capabilities.

**Installed Version:** 1.2026.3.195

**Location:** `C:\Users\60217257\.dotnet\tools\pacx.exe`

**Available Commands:**
- `pacx auth` - Manage authentication to Dataverse environments
- `pacx table` - Dataverse table manipulations
- `pacx column` - Column manipulations
- `pacx solution` - Work with Dataverse solutions
- `pacx plugin` - Plugin registration & step management
- `pacx workflow` - Activate/deactivate workflows
- `pacx webresources` - Manage web resources
- `pacx ribbon` - Ribbon customization
- `pacx forms` - Form manipulation
- `pacx project` - Project lifecycle management
- `pacx script` - Generate PACX scripts from Dataverse
- And many more...

**Setup Steps:**
1. Run `pacx auth` to configure authentication
2. Connect to your Dataverse environment
3. Use `pacx ...` commands directly; PACX is not an MCP server

**Documentation:** https://github.com/neronotte/Greg.Xrm.Command/wiki

---

### 3. Power Apps CLI (`powerplatform-pac`)
**Status:** ✅ PAC CLI installed & authenticated (MCP server disabled)

**What it does:** Official Microsoft Power Platform CLI for environment and solution management.

**Installed Version:** 2.5.1

**Location:** `C:\Users\60217257\.dotnet\tools\pac.exe`

**Current Authentication:** ✅ Active
- **User:** Dylan.Mordaunt@health.nsw.gov.au
- **Environment:** Illawarra Shoalhaven LHD
- **URL:** https://orgefc9aa3e.crm6.dynamics.com/
- **Cloud:** Public

**Available Commands:**
- `pac auth` - Authentication management
- `pac env` - Environment operations
- `pac solution` - Solution management
- `pac pcf` - Power Apps Component Framework
- `pac copilot mcp --run` - Start MCP server

---

## 🔧 Requires Configuration

### 4. Remote Power BI MCP Server (`powerbi-remote`)
**Status:** ⚠️ Not enabled in this repo

**What it does:** Remote MCP endpoint for querying Power BI semantic models via AI.

**Prerequisites:**
- Tenant admin must enable: "Users can use the Power BI Model Context Protocol server endpoint (Preview)"
- Users need **Build permissions** on target semantic models
- **Copilot license** required for `generate_query` tool

**Setup Steps:**
1. Ask your tenant admin to enable the MCP endpoint
2. Add it back to `.mcp.json` if you want to use it here
3. No additional configuration needed (uses your Microsoft account)

**Note:** Row-Level Security (RLS) is enforced for users, but NOT for service principals.

---

## 🔐 Authentication Summary

| Server | Auth Method | Setup Required |
|--------|-------------|----------------|
| Power BI Modeling | Azure AD Interactive | None (prompts on first use) |
| Power BI Remote | Microsoft Account | Tenant admin must enable |
| Power Platform PAC | Azure AD via PAC | Run `pac auth create` |
| PACX | Dataverse Auth | Run `pacx auth` |

---

## 🚀 Next Steps

### Immediate (No Additional Setup):
1. ✅ **Power BI Modeling MCP** - Already enabled, ready to use
2. ✅ **PAC CLI** - Already authenticated (Dylan.Mordaunt@health.nsw.gov.au)
3. ✅ **PACX** - Installed, run `pacx auth` to configure Dataverse access

### Requires Action:
1. **Remote Power BI** - Request tenant admin to enable MCP endpoint

### Optional Enhancements:
- Install .NET 8.0 x64 SDK (currently only runtime installed) for PACX development
- Setup service principal authentication for automated Power BI operations
- Configure environment variables for frequently used semantic model IDs

---

## 📝 Environment Variables Reference

For Power BI Modeling MCP service principal authentication:
```bash
AZURE_CLIENT_ID=<your-app-client-id>
AZURE_TENANT_ID=<your-tenant-id>
AZURE_CLIENT_SECRET=<your-client-secret>
# OR
AZURE_CLIENT_CERTIFICATE_PATH=<path-to-certificate>
AZURE_CLIENT_CERTIFICATE_PASSWORD=<cert-password-if-required>
```

For bypassing interactive login (Power BI):
```bash
PBI_MODELING_MCP_ACCESS_TOKEN=<pre-authenticated-token>
```

---

## 🔗 Documentation Links

- Power BI Modeling MCP: https://github.com/microsoft/powerbi-modeling-mcp
- Remote Power BI MCP: https://learn.microsoft.com/en-us/power-bi/developer/mcp/remote-mcp-server-tools
- Power Platform MCP: https://learn.microsoft.com/en-us/power-platform/developer/howto/use-mcp
- PAC CLI: https://learn.microsoft.com/en-us/power-platform/developer/cli/introduction
- PACX: https://github.com/neronotte/Greg.Xrm.Command

---

## 🛠️ Troubleshooting

### PACX not working
```bash
# Check if installed
dir %USERPROFILE%\.dotnet\tools\pacx.exe

# Reinstall if needed
"C:\Users\60217257\scoop\apps\dotnet-sdk\current\dotnet.exe" tool install -g Greg.Xrm.Command

# Check .NET 8.0 runtime
"C:\Users\60217257\scoop\apps\dotnet-sdk\current\dotnet.exe" --list-runtimes
```

### Power BI Modeling MCP issues
- Ensure you have access to Power BI/Fabric resources
- Check Azure AD authentication is working
- Verify Power BI permissions on semantic models

---

**Last Updated:** 8 April 2026
