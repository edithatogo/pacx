# Mirror Dataverse to OneLake

Use the Fabric commands to stage a Dataverse-to-OneLake workflow.

```powershell
pacx fabric workspace list
pacx fabric lakehouse create --workspace-id <workspace-id> --name DataverseMirror
pacx fabric link create --dataverse-env https://contoso.crm.dynamics.com --target-workspace <workspace-id>
pacx fabric link status
```

For lakehouse shortcuts:

```powershell
pacx onelake shortcut create `
  --workspace-id <workspace-id> `
  --item-id <lakehouse-id> `
  --source-type adlsGen2 `
  --source-path abfss://raw@storage.dfs.core.windows.net/account `
  --target-path Tables/account
```
