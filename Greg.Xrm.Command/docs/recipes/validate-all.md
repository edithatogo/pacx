# Validate All

Use `pacx validate all` when you want a single preflight command for command reference parity, command-page content checks, and catalog contract checks.

```powershell
pacx validate all
```

To point the validation at a different repository root, pass `--catalog-root`:

```powershell
pacx validate all --catalog-root .
```

This is the command to run in CI before publishing generated docs or catalog-backed capability surfaces.
