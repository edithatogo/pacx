# Deploy a Solution From CI

Use PACX in CI when the workflow needs repeatable validation, clear logs, and non-interactive credentials.

## Prerequisites

- PACX CLI installed (`dotnet tool install -g Greg.Xrm.Command`)
- Service principal or managed identity with Solution Import role
- Solution package (.zip) built by `pacx package build`

## CI Job Steps

### 1. Restore tools and dependencies

```powershell
dotnet tool restore
```

### 2. Authenticate with least-privilege credentials

```powershell
pacx auth create --name ci-env --url https://${{ vars.ENV_URL }} --client-id ${{ secrets.CLIENT_ID }} --client-secret ${{ secrets.CLIENT_SECRET }} --tenant ${{ secrets.TENANT_ID }}
```

### 3. Validate the solution package before importing

```powershell
pacx solution diff --file my-solution.zip --env ci-env
pacx quality gate --env ci-env
```

### 4. Import the solution to the target environment

```powershell
pacx package deploy --env ci-env --zip my-solution.zip
```

### 5. Publish customizations and run smoke checks

```powershell
pacx publish all --env ci-env
pacx connection-list --env ci-env
```

## Expected Output

```
> pacx solution diff --file my-solution.zip --env ci-env
Components added: 12
Components updated: 3
Components removed: 0

> pacx package deploy --env ci-env --zip my-solution.zip
Deploying my-solution.zip... OK
Import completed successfully (00:01:23)
```

## GitHub Actions Integration

```yaml
- name: Deploy solution
  run: |
    pacx auth create --name ci-env --url ${{ vars.ENV_URL }} --client-id ${{ secrets.CLIENT_ID }} --client-secret ${{ secrets.CLIENT_SECRET }} --tenant ${{ secrets.TENANT_ID }}
    pacx solution diff --file my-solution.zip --env ci-env
    pacx package deploy --env ci-env --zip my-solution.zip
    pacx publish all --env ci-env
```
