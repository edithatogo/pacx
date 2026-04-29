# Validate Power Fx

Use `power-fx validate` to check expressions before packaging or import.

```powershell
pacx power-fx validate --expression "If(Amount > 100, \"Review\", \"Auto\")"
pacx power-fx validate --file .\rules.json
pacx power-fx validate --expression "Name & \" - \" & AccountNumber" --table account
```

Format expressions with:

```powershell
pacx power-fx format --expression "If(
 true,   1 )"
pacx power-fx format --file .\formula.fx --in-place
```
