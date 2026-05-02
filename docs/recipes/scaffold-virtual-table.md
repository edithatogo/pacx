# Scaffold a Virtual Table

Use virtual table scaffolding to keep Dataverse metadata and external data contracts aligned.

## Prerequisites

- PACX CLI installed (`dotnet tool install -g Greg.Xrm.Command`)
- Dataverse environment with virtual table support enabled
- Connection profile created (`pacx auth create`)

## Workflow

### 1. Identify the external source schema

Determine the external data source, connection reference, and columns required. For example, a SQL Server view or an Azure Data Lake file.

### 2. Generate the virtual table scaffold

```powershell
pacx virtual-table scaffold --env myenv --name "ExternalOrder" --external-name "dbo.vw_orders" --data-source "SQL Source"
```

### 3. Review generated metadata before applying

```powershell
pacx table print --env myenv --name "pac_ExternalOrder"
```

### 4. Customize and apply columns

```powershell
pacx column add string --env myenv --table-name "pac_ExternalOrder" --name "OrderNumber" --logical-name "pac_ordernumber" --required
pacx column add decimal --env myenv --table-name "pac_ExternalOrder" --name "TotalAmount" --logical-name "pac_totalamount"
```

### 5. Commit the scaffold so future changes are reviewable

```powershell
pacx table exportMetadata --env myenv --name "pac_ExternalOrder" --output ./metadata
```

## Expected Output

```
> pacx virtual-table scaffold --env myenv --name "ExternalOrder"
Generating scaffold for ExternalOrder
  External name: dbo.vw_orders
  Data source: SQL Source
  Columns detected:
    - OrderNumber (string)
    - TotalAmount (decimal)
    - OrderDate (datetime)
Scaffold generated. Table logical name: pac_externalorder
```

## CI/CD Integration

Include the scaffold and export commands in your pipeline to detect drift between the external schema and Dataverse metadata:

```powershell
pacx virtual-table scaffold --env ci-env --name "ExternalOrder"
pacx table exportMetadata --env ci-env --name "pac_ExternalOrder"
```
