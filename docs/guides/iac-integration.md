# IaC Integration

Integrate PACX with infrastructure-as-code workflows for Power Platform environment and solution lifecycle management.

## Bicep

PACX can be invoked from a Bicep deployment script via the Azure CLI or a Docker step to manage Power Platform resources that complement Azure infrastructure.

### Example: Environment creation after Bicep deployment

```bicep
param environmentName string
param location string = 'eastus'

resource storage 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: 'st${uniqueString(resourceGroup().id)}'
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}
```

Trigger PACX post-deployment:

```bash
# deploy.sh
az deployment group create \
  --resource-group my-rg \
  --template-file main.bicep \
  --parameters environmentName=myenv

pacx env create \
  --name "Dev-$environmentName" \
  --type Sandbox \
  --region unitedstates \
  --wait
```

### Using deployment scripts (Azure CLI)

```bash
az deployment group create \
  --resource-group my-rg \
  --template-file infra.bicep \
  --query properties.outputs

pacx solution list --format json | \
  jq '.[] | select(.ismanaged == false) | {name, version}'
```

## Terraform

PACX complements Terraform-managed Azure infrastructure with Power Platform lifecycle actions.

### Example: Provision Azure resources + create Power Platform environment

```hcl
resource "azurerm_resource_group" "main" {
  name     = "pacx-demo-rg"
  location = "East US"
}

resource "azurerm_storage_account" "main" {
  name                     = "pacxdemostorage"
  resource_group_name      = azurerm_resource_group.main.name
  location                 = azurerm_resource_group.main.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}
```

Post-provision, use a `null_resource` with a local-exec provisioner:

```hcl
resource "null_resource" "pacx_env" {
  depends_on = [azurerm_storage_account.main]

  provisioner "local-exec" {
    command = "pacx env create --name \"${var.environment_name}\" --type Sandbox --wait"
    environment = {
      MSAL_CLIENT_ID     = var.msal_client_id
      MSAL_CLIENT_SECRET = var.msal_client_secret
      TENANT_ID          = var.tenant_id
    }
  }
}
```

### Solution import after infrastructure is ready

```hcl
resource "null_resource" "pacx_solution" {
  depends_on = [null_resource.pacx_env]

  provisioner "local-exec" {
    command = "pacx solution import --solution-id \"${var.solution_id}\""
  }
}
```

## Docker-based IaC pipelines

When running in ephemeral CI runners, use the PACX Docker image directly:

```yaml
services:
  pacx:
    image: ghcr.io/edithatogo/greg.xrm.command:latest
    environment:
      MSAL_CLIENT_ID: ${MSAL_CLIENT_ID}
      MSAL_CLIENT_SECRET: ${MSAL_CLIENT_SECRET}
      TENANT_ID: ${TENANT_ID}
```

Then alias PACX in your pipeline:

```bash
alias pacx='docker run --rm -e MSAL_CLIENT_ID -e MSAL_CLIENT_SECRET -e TENANT_ID ghcr.io/edithatogo/greg.xrm.command'
```

Combine with Terraform:

```bash
terraform init && terraform apply -auto-approve
pacx env create --name "prod-$(terraform output -raw env_suffix)" --type Production
pacx solution import --solution-id "$SOLUTION_ID"
```

## Best practices

- Use least-privilege service principals for PACX authentication in IaC pipelines.
- Set `--wait` on environment create/import to ensure sequential dependencies.
- Store MSAL credentials in a vault (Azure Key Vault, GitHub Secrets, Azure DevOps Variable Groups).
- Run `pacx solution list --format json` before destructive Terraform operations to validate solution state.
