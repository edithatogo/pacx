# Implementation Plan: Code Quality Automation

## Anti-Stub Preamble
No task is complete without a **verifiable artifact**: a cloud scan result, a build pass, a removed `using` line, or a green CI check. `/conductor-review` runs at every phase boundary. Any stub, TODO, or "future work" comment causes immediate rejection.

## Overview
Add cloud-hosted code quality analysis (SonarCloud), API compatibility gates (Microsoft.DotNet.ApiCompat), eliminate redundant code (implicit usings), and harden pre-commit hooks (Husky.NET).

## Phase Structure
Each phase: Analyze → Implement → Verify → Auto-Review → Proceed.

## Phases

### Phase 1: SonarCloud Integration

**Step 1: Analyze**
- Read all existing workflow files to understand CI structure
- Determine if SonarCloud project already exists (check for `sonar-project.properties`)
- Read `ci.yml` `coverage` job to understand where to integrate
- Read SonarCloud docs for GitHub Action integration

**Step 2: Implement**

Create **`.github/workflows/sonarcloud.yml`**:
```yaml
name: SonarCloud Analysis
on:
  push:
    branches: [ master, main ]
  pull_request:
    branches: [ master, main ]
    types: [opened, synchronize, reopened]

permissions:
  contents: read
  security-events: write

jobs:
  analyze:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      
      - name: Restore
        run: dotnet restore Greg.Xrm.Command/Greg.Xrm.Command.sln
      
      - name: Build
        run: dotnet build Greg.Xrm.Command/Greg.Xrm.Command.sln --no-restore
      
      - name: SonarCloud Scan
        uses: SonarSource/sonarcloud-github-action@v3
        env:
          SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
        with:
          args: >
            -Dsonar.organization=your-org
            -Dsonar.projectKey=your-org_pacx
            -Dsonar.projectName=PACX
            -Dsonar.cs.opencover.reportsPaths=**/coverage.opencover.xml
            -Dsonar.coverage.exclusions=**/*Test*.cs
            -Dsonar.exclusions=**/*.g.cs,**/obj/**
```

Update **`sonar-project.properties`** (create if not exists):
```properties
sonar.organization=your-org
sonar.projectKey=your-org_pacx
sonar.projectName=PACX
sonar.language=cs
sonar.sourceEncoding=UTF-8
sonar.cs.opencover.reportsPaths=**/coverage.opencover.xml
sonar.coverage.exclusions=**/*Test*.cs,**/obj/**
sonar.exclusions=**/*.g.cs
```

**Step 3: Anti-Stub Verification**
- [ ] SonarCloud workflow created — verified by file existence
- [ ] `SONAR_TOKEN` secret documented as required in setup — verified
- [ ] Coverage reports fed into SonarCloud — verified by args
- [ ] Exclusions configured (test files, generated code) — verified
- [ ] Quality gate blocks PR on new issues — verified by SonarCloud defaults

**Step 4: Auto-Review**
Run `/conductor-review`. If review flags:
- Missing secrets → document as prerequisite
- Incorrect paths → fix project key/organization
- Coverage report path wrong → fix regex

**Step 5: Proceed**
Phase complete → move to Phase 2.

### Phase 2: API Compatibility

**Step 1: Analyze**
- Check current public API surface by examining interfaces in `Greg.Xrm.Command.Interfaces/`
- Read `Microsoft.DotNet.ApiCompat` docs
- Check if there's an existing baseline assembly

**Step 2: Implement**

Add to **`Directory.Build.props`** (create at repo root if not exists):
```xml
<Project>
  <PropertyGroup>
    <TargetFrameworks>net10.0;net11.0</TargetFrameworks>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup Condition="'$(MSBuildProjectName)' != 'Greg.Xrm.Command.Core.TestSuite'">
    <PackageReference Include="Microsoft.DotNet.ApiCompat" Version="9.0.0" PrivateAssets="all" />
  </ItemGroup>
</Project>
```

Create baseline assembly:
```bash
# Build the interfaces project to generate the baseline
dotnet build Greg.Xrm.Command/Greg.Xrm.Command.Interfaces/Greg.Xrm.Command.Interfaces.csproj
# Copy the output DLL as baseline
cp Greg.Xrm.Command/Greg.Xrm.Command.Interfaces/bin/Release/net10.0/Greg.Xrm.Command.Interfaces.dll \
   .api-compat/baseline/
```

Create **`.github/workflows/api-compat.yml`**:
```yaml
name: API Compatibility
on:
  pull_request:
    paths:
      - 'Greg.Xrm.Command/Greg.Xrm.Command.Interfaces/**'

jobs:
  check:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      - name: Check API compatibility
        run: |
          dotnet build Greg.Xrm.Command/Greg.Xrm.Command.Interfaces/Greg.Xrm.Command.Interfaces.csproj
          dotnet tool run api-compat \
            --baseline .api-compat/baseline/Greg.Xrm.Command.Interfaces.dll \
            --implementation Greg.Xrm.Command/Greg.Xrm.Command.Interfaces/bin/Release/net10.0/Greg.Xrm.Command.Interfaces.dll
```

Create **`.api-compat/README.md`**:
```markdown
# API Compatibility Baseline

This directory contains baseline assemblies for API compatibility checking.
When a deliberate breaking change is made:
1. Rebuild the interfaces project
2. Copy the new DLL here
3. Commit with a message explaining the breaking change
```

**Step 3: Anti-Stub Verification**
- [ ] `Microsoft.DotNet.ApiCompat` referenced in Directory.Build.props — verified
- [ ] Baseline assembly captured — verified by file existence
- [ ] CI workflow created for API compat — verified
- [ ] Breaking changes documented in ADR when needed — verified by process

**Step 4: Auto-Review**
Run `/conductor-review`.

**Step 5: Proceed**
Phase complete → move to Phase 3.

### Phase 3: Implicit Usings

**Step 1: Analyze**
- Count current `using` statements across the codebase (baseline: ~500+ across ~300 files)
- Read each project's csproj to check existing `<ImplicitUsings>` setting
- Identify which global usings would be enabled by default

**Step 2: Implement**

Add to the repo root **`Directory.Build.props`** (or update existing):
```xml
<Project>
  <PropertyGroup>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

Default implicit usings for .NET projects include:
- `System`
- `System.Collections.Generic`
- `System.IO`
- `System.Linq`
- `System.Net.Http`
- `System.Threading`
- `System.Threading.Tasks`
- `System.Text.Json` (for web SDK)

**Remove redundant `using` statements** by running:
```bash
# Use dotnet-format to clean up redundant usings
dotnet format Greg.Xrm.Command/Greg.Xrm.Command.sln --severity info --diagnostic IDE0005
```

Or use `dotnet build` with warnings-as-errors for IDE0005 to catch redundant usings.

Verify removal by checking that the following usings are removed from individual files (they're now global):
- `using System;`
- `using System.Collections.Generic;`
- `using System.IO;`
- `using System.Linq;`
- `using System.Net.Http;`
- `using System.Threading;`
- `using System.Threading.Tasks;`

Do NOT remove:
- `using Greg.Xrm.Command.*` (project namespace — manual)
- `using Microsoft.Xrm.Sdk.*` (external namespace — manual)
- `using System.Text.Json` (only implicit in web SDK, not console)

**Step 3: Anti-Stub Verification**
- [ ] `Directory.Build.props` created/updated — verified by file read
- [ ] `ImplicitUsings` set to `enable` — verified
- [ ] Build passes after removing redundant usings — verified (conceptually: no missing type errors)
- [ ] No needed usings removed — verified by checking that external namespaces (Microsoft.Xrm.*) still have explicit usings

**Step 4: Auto-Review**
Run `/conductor-review`.

**Step 5: Proceed**
Phase complete → move to Phase 4.

### Phase 4: Enhanced Pre-commit Hooks

**Step 1: Analyze**
- Read existing Husky.NET configuration (check for `.husky/` or `husky.config.js`)
- Read `.commitlintrc.json` for commit message format
- Check current hooks (pre-commit, pre-push, commit-msg)

**Step 2: Implement**

If Husky.NET is installed, add/update hooks:

**`.husky/pre-commit`:**
```bash
#!/bin/sh
# Run code formatting check
dotnet format Greg.Xrm.Command/Greg.Xrm.Command.sln --verify-no-changes --severity warn
if [ $? -ne 0 ]; then
  echo "Code formatting issues found. Run 'dotnet format' to fix."
  exit 1
fi

# Build solution (quick check)
dotnet build Greg.Xrm.Command/Greg.Xrm.Command.sln --no-restore --verbosity minimal
if [ $? -ne 0 ]; then
  echo "Build failed. Fix errors before committing."
  exit 1
fi
```

**`.husky/pre-push`:**
```bash
#!/bin/sh
# Run tests before pushing
dotnet test Greg.Xrm.Command/Greg.Xrm.Command.Core.TestSuite/Greg.Xrm.Command.Core.TestSuite.csproj \
  --no-build --verbosity minimal --filter "TestCategory!=Integration"
if [ $? -ne 0 ]; then
  echo "Tests failed. Fix before pushing."
  exit 1
fi
```

**`.husky/commit-msg`:**
This is already set up by commitlint. Verify it exists:
```bash
#!/bin/sh
npx --no -- commitlint --edit "$1"
```

**Step 3: Anti-Stub Verification**
- [ ] Pre-commit hook runs format check + build — verified by reading hook file
- [ ] Pre-push hook runs unit tests — verified by reading hook file
- [ ] Commit-msg hook runs commitlint — verified by reading hook file
- [ ] All hooks have proper exit codes — verified (non-zero on failure)
- [ ] Hook setup documented in CONTRIBUTING.md — verified

**Step 4: Auto-Review**
Run `/conductor-review`.

**Step 5: Proceed**
Track complete.

## Rollback Plan
If any change breaks the build or workflow:
1. Revert the specific file change
2. Document the issue
3. Re-approach with a fix
4. Re-run `/conductor-review` before proceeding
