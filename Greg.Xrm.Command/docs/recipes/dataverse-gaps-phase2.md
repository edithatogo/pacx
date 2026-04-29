# Dataverse Platform Gaps Phase 2

Use these commands for Dataverse platform features that do not fit the older workflow, table, column, or security command groups.

```powershell
pacx business-rule list --table account
pacx business-rule export --id <workflow-id> --file .\business-rule.json
pacx business-rule import --file .\business-rule.json --table account
pacx business-rule activate --id <workflow-id>
pacx business-rule deactivate --id <workflow-id>
```

```powershell
pacx bpf list
pacx bpf export --id <workflow-id> --file .\bpf.json
pacx bpf import --file .\bpf.json
pacx bpf activate --id <workflow-id>
```

```powershell
pacx ddr list
pacx ddr run --rule-id <duplicate-rule-id>
pacx ddr enable --rule-id <duplicate-rule-id>
pacx ddr disable --rule-id <duplicate-rule-id>
```

```powershell
pacx audit status --env dev
pacx audit enable-table --name account
pacx audit export --table account --since 2026-01-01T00:00:00Z --format jsonl --file .\audit.jsonl
```

```powershell
pacx fsp list
pacx fsp apply --profile-id <profile-id> --user-or-team-id <principal-id>
pacx fsp bulk-assign --profile-id <profile-id> --file .\principals.csv
```

```powershell
pacx endpoint list
pacx endpoint register --url https://example.test/hook --auth WebhookKey --name contoso-hook
pacx endpoint delete --id <service-endpoint-id>
```

Alternate keys are already covered by the existing key command group:

```powershell
pacx key create --table account --columns accountnumber,telephone1
```

File column upload and rollup recalculation use the column command group:

```powershell
pacx column file-upload --table account --id <record-id> --column new_file --file .\document.pdf
pacx column rollup-recalculate --table account --id <record-id> --column new_total
```
