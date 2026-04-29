# Manage Power BI Datasets

Use the Power BI command group for workspace-level dataset operations.

```powershell
pacx dataset list --workspace-id <workspace-id>
pacx dataset publish --workspace-id <workspace-id> --pbix .\Reports\Sales.pbix --name Sales
pacx dataset refresh trigger --workspace-id <workspace-id> --id <dataset-id>
pacx dataset refresh status --workspace-id <workspace-id> --id <dataset-id>
```

Deployment pipelines and capacity operations are also available:

```powershell
pacx pipeline list
pacx pipeline stage-assign --pipeline-id <pipeline-id> --stage dev --workspace-id <workspace-id>
pacx pipeline deploy --pipeline-id <pipeline-id> --source-stage dev --target-stage test
pacx capacity list
pacx capacity workspace-assign --capacity-id <capacity-id> --workspace-id <workspace-id>
```
