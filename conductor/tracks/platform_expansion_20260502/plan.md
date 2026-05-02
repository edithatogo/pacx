# Implementation Plan: Platform Expansion

## Anti-Stub Preamble
Every task produces a **published artifact**: a Docker image in a registry, a PowerShell module on PSGallery, a VS Code extension published, a GitHub Action in the Marketplace, or an Azure DevOps extension. If it cannot be published, it is documented with clear setup steps. `/conductor-review` runs at every phase boundary. No task is complete without verification.

## Overview
Package and distribute PACX across multiple platforms and ecosystems. Each platform target is a self-contained deliverable with its own build, test, and publish pipeline.

## Phase Structure
Each phase: Analyze → Implement → Verify → Auto-Review → Proceed.

## Phases

### Phase 1: Docker Image

**Step 1: Analyze**
- Check existing `Dockerfile` if any
- Read `release.yml` to understand current build/publish flow
- Check GitHub Container Registry access (ghcr.io)
- Determine .NET SDK versions to use in multi-stage build

**Step 2: Implement**

Create **`Dockerfile`** at repo root:
```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Greg.Xrm.Command/ .
RUN dotnet restore Greg.Xrm.Command.sln
RUN dotnet publish Greg.Xrm.Command/Greg.Xrm.Command.csproj \
    -c Release \
    -o /app \
    --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Greg.Xrm.Command.dll"]
```

Create **`.github/workflows/docker-publish.yml`**:
```yaml
name: Docker Publish
on:
  release:
    types: [published]
  workflow_dispatch:

permissions:
  contents: read
  packages: write

jobs:
  build-and-push:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v3

      - name: Log in to GitHub Container Registry
        uses: docker/login-action@v3
        with:
          registry: ghcr.io
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: Extract metadata
        id: meta
        uses: docker/metadata-action@v5
        with:
          images: ghcr.io/${{ github.repository }}
          tags: |
            type=semver,pattern={{version}}
            type=semver,pattern={{major}}.{{minor}}
            type=sha

      - name: Build and push
        uses: docker/build-push-action@v6
        with:
          context: .
          push: true
          tags: ${{ steps.meta.outputs.tags }}
          labels: ${{ steps.meta.outputs.labels }}
          cache-from: type=gha
          cache-to: type=gha,mode=max
```

Create **`docker-compose.yml`** for CI/CD example usage:
```yaml
version: '3.8'
services:
  pacx:
    image: ghcr.io/your-org/pacx:latest
    environment:
      - MSAL_CLIENT_ID=${MSAL_CLIENT_ID}
      - MSAL_CLIENT_SECRET=${MSAL_CLIENT_SECRET}
    volumes:
      - ./output:/app/output
    command: ["forms", "list", "--tenant", "${TENANT_ID}"]
```

**Step 3: Anti-Stub Verification**
- [ ] `Dockerfile` created with multi-stage build — verified by file existence
- [ ] Docker publish workflow created — verified by file existence
- [ ] Workflow triggers on release publish + manual dispatch — verified
- [ ] GitHub Container Registry push configured — verified by registry URL
- [ ] Semantic version tags applied — verified by metadata-action config
- [ ] Docker Compose example created — verified by file existence

**Step 4: Auto-Review**
Run `/conductor-review`.

**Step 5: Proceed**
Phase complete → move to Phase 2.

### Phase 2: PowerShell Module

**Step 1: Analyze**
- Read existing `docs/guides/forms-powershell.md` to understand current PowerShell docs
- Check if any PowerShell module project exists
- Read NuGet packaging pattern used in `Greg.Xrm.Command.csproj`

**Step 2: Implement**

Create **`tools/pacx-powershell/pacx.psd1`** — PowerShell module manifest:
```powershell
@{
  RootModule        = 'pacx.psm1'
  ModuleVersion     = '1.0.0'
  GUID              = 'xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx'
  Author            = 'PACX Team'
  CompanyName       = 'PACX'
  Copyright         = '(c) PACX Team. All rights reserved.'
  Description       = 'PowerShell module for PACX — Dataverse and Power Platform management'
  PowerShellVersion = '7.0'
  FunctionsToExport = @(
    'Get-PacxForm'
    'Get-PacxFormResponse'
    'Invoke-PacxSolution'
    'Get-PacxEnvironment'
    'New-PacxEnvironment'
  )
  CmdletsToExport   = @()
  AliasesToExport   = @()
  PrivateData       = @{
    PSData = @{
      Tags       = @('PACX', 'Dataverse', 'PowerPlatform', 'Dynamics365')
      ProjectUri = 'https://github.com/your-org/pacx'
      LicenseUri = 'https://github.com/your-org/pacx/blob/main/LICENSE'
    }
  }
}
```

Create **`tools/pacx-powershell/pacx.psm1`**:
```powershell
function Get-PacxForm {
  <#
  .SYNOPSIS
    Lists Microsoft Forms
  .EXAMPLE
    Get-PacxForm -Tenant contoso.onmicrosoft.com
  #>
  [CmdletBinding()]
  param(
    [Parameter(Mandatory)] [string]$Tenant,
    [string]$Owner,
    [switch]$Json
  )
  $args = @('forms', 'list', '--tenant', $Tenant)
  if ($Owner) { $args += '--owner'; $args += $Owner }
  if ($Json)  { $args += '--format'; $args += 'json' }
  & pacx @args
}

function Get-PacxFormResponse {
  <#
  .SYNOPSIS
    Gets response count for a Microsoft Form
  #>
  [CmdletBinding()]
  param(
    [Parameter(Mandatory)] [string]$Tenant,
    [Parameter(Mandatory)] [string]$FormId
  )
  & pacx forms response count --tenant $Tenant --form-id $FormId
}

function Invoke-PacxSolution {
  <#
  .SYNOPSIS
    Imports or exports a Dataverse solution
  #>
  [CmdletBinding()]
  param(
    [Parameter(Mandatory)] [string]$Action,
    [Parameter(Mandatory)] [string]$SolutionName
  )
  & pacx solution $Action --name $SolutionName
}

function Get-PacxEnvironment {
  [CmdletBinding()]
  param([switch]$Json)
  $args = @('env', 'list')
  if ($Json) { $args += '--format'; $args += 'json' }
  & pacx @args
}

function New-PacxEnvironment {
  [CmdletBinding()]
  param(
    [Parameter(Mandatory)] [string]$Name,
    [string]$Type = 'Sandbox',
    [string]$Region,
    [switch]$Wait
  )
  $args = @('env', 'create', '--name', $Name, '--type', $Type)
  if ($Region) { $args += '--region'; $args += $Region }
  if ($Wait)   { $args += '--wait' }
  & pacx @args
}

Export-ModuleMember -Function @(
  'Get-PacxForm', 'Get-PacxFormResponse',
  'Invoke-PacxSolution',
  'Get-PacxEnvironment', 'New-PacxEnvironment'
)
```

Step to publish (in `release.yml`):
```yaml
      - name: Publish PowerShell module
        if: success()
        env:
          NUGET_API_KEY: ${{ secrets.NUGET_API_KEY }}
        run: |
          # Publish to PowerShell Gallery
          Publish-Module -Path tools/pacx-powershell -NuGetApiKey $env:NUGET_API_KEY
```

**Step 3: Anti-Stub Verification**
- [ ] Module manifest created — verified by file existence
- [ ] Module script created with 5+ functions — verified
- [ ] Each function wraps a real `pacx` command — verified by comparing to command registry
- [ ] Publish step in release workflow — verified
- [ ] Module documented — verified by reading docs

**Step 4: Auto-Review**
Run `/conductor-review`.

**Step 5: Proceed**
Phase complete → move to Phase 3.

### Phase 3: VS Code Extension

**Step 1: Analyze**
- Check if `tools/vscode-extension/` directory exists
- Read VS Code extension API docs for command palette + tree view

**Step 2: Implement**

Scaffold a VS Code extension at **`tools/vscode-extension/`**:

**`package.json`:**
```json
{
  "name": "pacx-vscode",
  "displayName": "PACX for VS Code",
  "description": "Dataverse and Power Platform management from VS Code",
  "version": "1.0.0",
  "publisher": "pacx",
  "engines": { "vscode": "^1.85.0" },
  "categories": ["Other"],
  "activationEvents": [],
  "main": "./out/extension.js",
  "contributes": {
    "commands": [
      { "command": "pacx.listForms", "title": "PACX: List Forms" },
      { "command": "pacx.listSolutions", "title": "PACX: List Solutions" },
      { "command": "pacx.listEnvironments", "title": "PACX: List Environments" },
      { "command": "pacx.runCommand", "title": "PACX: Run Command..." }
    ],
    "viewsContainers": {
      "activitybar": [
        { "id": "pacx-explorer", "title": "PACX", "icon": "media/pacx.svg" }
      ]
    },
    "views": {
      "pacx-explorer": [
        { "id": "pacx.forms", "name": "Forms" },
        { "id": "pacx.solutions", "name": "Solutions" }
      ]
    },
    "configuration": {
      "title": "PACX",
      "properties": {
        "pacx.cliPath": { "type": "string", "default": "pacx", "description": "Path to the pacx CLI" }
      }
    }
  }
}
```

**`src/extension.ts`:**
```typescript
import * as vscode from 'vscode';
import { exec } from 'child_process';

function runPacx(args: string[]): Promise<string> {
  const config = vscode.workspace.getConfiguration('pacx');
  const cliPath = config.get<string>('cliPath', 'pacx');
  return new Promise((resolve, reject) => {
    exec(`${cliPath} ${args.join(' ')}`, (err, stdout, stderr) => {
      if (err) reject(stderr);
      else resolve(stdout);
    });
  });
}

export function activate(context: vscode.ExtensionContext) {
  context.subscriptions.push(
    vscode.commands.registerCommand('pacx.listForms', async () => {
      const result = await runPacx(['forms', 'list', '--format', 'json']);
      // Display as tree view or quick pick
    }),
    vscode.commands.registerCommand('pacx.listSolutions', async () => {
      const result = await runPacx(['solution', 'list', '--format', 'json']);
    }),
    vscode.commands.registerCommand('pacx.listEnvironments', async () => {
      const result = await runPacx(['env', 'list', '--format', 'json']);
    }),
    vscode.commands.registerCommand('pacx.runCommand', async () => {
      const command = await vscode.window.showInputBox({ prompt: 'Enter pacx command' });
      if (command) {
        const result = await runPacx(command.split(' '));
        vscode.window.showInformationMessage(result);
      }
    })
  );
}
```

Add publish step in **`release.yml`**:
```yaml
      - name: Publish VS Code extension
        if: success()
        run: |
          cd tools/vscode-extension
          npm install
          npx vsce publish -p ${{ secrets.VSCE_TOKEN }}
```

**Step 3: Anti-Stub Verification**
- [ ] Extension scaffolded with package.json — verified by file existence
- [ ] At least 4 commands registered — verified by reading package.json
- [ ] Tree view provider configured — verified
- [ ] Publish step in release workflow — verified

**Step 4: Auto-Review**
Run `/conductor-review`.

**Step 5: Proceed**
Phase complete → move to Phase 4.

### Phase 4: GitHub Action

**Step 1: Analyze**
- Read Docker publish workflow from Phase 1
- Read GitHub Action docs for creating custom actions

**Step 2: Implement**

Create **`action.yml`** at repo root:
```yaml
name: 'PACX CLI Action'
description: 'Run PACX CLI commands in your workflow'
author: 'PACX Team'
branding:
  icon: 'terminal'
  color: 'blue'

inputs:
  command:
    description: 'PACX CLI command and arguments'
    required: true
  working-directory:
    description: 'Working directory'
    default: '.'

runs:
  using: 'docker'
  image: 'docker://ghcr.io/${{ github.repository }}:latest'
  args:
    - ${{ inputs.command }}
```

Add to **`release.yml`**: Publish action to Marketplace:
```yaml
      - name: Publish GitHub Action
        if: success()
        run: |
          # The action is published automatically when pushed to main
          # with a valid action.yml in the root
          echo "GitHub Action published from action.yml"
```

**Step 3: Anti-Stub Verification**
- [ ] `action.yml` created with docker-based runner — verified
- [ ] Action uses ghcr.io image from Phase 1 — verified by image reference
- [ ] Inputs defined: command, working-directory — verified
- [ ] Branding configured: icon, color — verified

**Step 4: Auto-Review**
Run `/conductor-review`.

**Step 5: Proceed**
Phase complete → move to Phase 5.

### Phase 5: Azure DevOps Task

**Step 1: Analyze**
- Read Azure DevOps extension docs
- Check `vss-extension.json` requirements

**Step 2: Implement**

Create **`tools/azure-devops-extension/`**:

**`vss-extension.json`:**
```json
{
  "manifestVersion": 1,
  "id": "pacx-azure-devops",
  "version": "1.0.0",
  "name": "PACX CLI Tasks",
  "description": "Azure DevOps tasks for PACX CLI commands",
  "publisher": "pacx",
  "targets": [{ "id": "Microsoft.VisualStudio.Services" }],
  "categories": ["Azure Pipelines"],
  "icons": { "default": "images/icon.png" },
  "files": [
    { "path": "tasks" }
  ],
  "contributions": [
    {
      "id": "pacx-run",
      "type": "ms.vss-distributed-task.task",
      "targets": ["ms.vss-distributed-task.tasks"],
      "properties": {
        "name": "tasks/pacx-run"
      }
    }
  ]
}
```

Create **`tasks/pacx-run/task.json`:**
```json
{
  "id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "name": "PacxCliRun",
  "friendlyName": "PACX CLI Run",
  "description": "Run a PACX CLI command",
  "helpMarkDown": "Run any pacx command in your pipeline",
  "category": "Utility",
  "author": "PACX Team",
  "version": { "Major": 1, "Minor": 0, "Patch": 0 },
  "instanceNameFormat": "Run pacx $(command)",
  "inputs": [
    {
      "name": "command",
      "type": "string",
      "label": "Command",
      "required": true,
      "helpMarkDown": "Full pacx command (e.g., 'forms list --tenant example.onmicrosoft.com')"
    },
    {
      "name": "workingDirectory",
      "type": "string",
      "label": "Working directory",
      "defaultValue": "",
      "required": false
    }
  ],
  "execution": {
    "Node": {
      "target": "index.js"
    }
  }
}
```

Add publish step in **`release.yml`**:
```yaml
      - name: Publish Azure DevOps extension
        if: success()
        run: |
          cd tools/azure-devops-extension
          npx tfx extension create --manifest-globs vss-extension.json
          npx tfx extension publish --token ${{ secrets.AZURE_DEVOPS_TOKEN }}
```

**Step 3: Anti-Stub Verification**
- [ ] Extension manifest created — verified by file existence
- [ ] Task definition created with inputs — verified
- [ ] Publish step configured — verified

**Step 4: Auto-Review**
Run `/conductor-review`.

**Step 5: Proceed**
Phase complete → move to Phase 6.

### Phase 6: IaC Provider

**Step 1: Analyze**
- Read Bicep `provider` documentation
- Read Terraform provider development docs

**Step 2: Implement**

Create **`docs/guides/iac-integration.md`**:
```markdown
# IaC Integration with PACX

## Bicep
Bicep can manage Power Platform resources via the `provider` keyword:
```bicep
provider 'microsoft.powerplatform' '@preview'
```

## Terraform
The Terraform Power Platform provider:
```hcl
terraform {
  required_providers {
    powerplatform = {
      source = "microsoft/powerplatform"
    }
  }
}
provider "powerplatform" {}
```

## PACX as Provisioning Script
Use PACX for post-provisioning automation:
```bash
# After Bicep/Terraform creates the environment:
pacx solution import --name my_solution.zip
pacx env set-settings --environment $ENV_ID --settings-file settings.json
```
```

**Step 3: Anti-Stub Verification**
- [ ] IaC integration guide created — verified by file existence
- [ ] Bicep example included — verified
- [ ] Terraform example included — verified
- [ ] PACX post-provisioning examples included — verified

**Step 4: Auto-Review**
Run `/conductor-review`.

**Step 5: Proceed**
Track complete.

## Rollback Plan
Any phase that cannot publish:
1. Skip the publish step
2. Document the publish instructions in a README
3. The phase is still valid (code exists)
4. Re-attempt publish when credentials/permissions are available
