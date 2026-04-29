# Desktop Flow RPA

Use the desktop-flow commands to inspect and run Power Automate Desktop flows.

```powershell
pacx desktop-flow list --env <environment-id>
pacx desktop-flow get --id <workflow-id>
pacx desktop-flow trigger --env <environment-id> --id <workflow-id> --machine-group <group> --input account=Contoso
pacx desktop-flow run list --env <environment-id> --id <workflow-id>
```

Machines, machine groups, and approvals are available through:

```powershell
pacx desktop-flow machine list --env <environment-id>
pacx desktop-flow machine-group list --env <environment-id>
pacx approval list --env <environment-id>
pacx approval respond --env <environment-id> --id <approval-id> --decision approve --comment "Reviewed"
```
