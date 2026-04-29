# Validate Connectors in CI

Use `pacx connector validate` before importing connector definitions into Dataverse.

```yaml
name: Validate connectors

on:
  pull_request:
    paths:
      - connectors/**/*.json

jobs:
  validate:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v5
        with:
          dotnet-version: 11.0.100-preview.3.26207.106
      - run: dotnet tool restore
      - run: dotnet build Greg.Xrm.Command/Greg.Xrm.Command.sln --configuration Release
      - name: Validate connector definitions
        shell: bash
        run: |
          for file in connectors/*.json; do
            dotnet run --project Greg.Xrm.Command/Greg.Xrm.Command.Core -- connector validate --file "$file" --strict
          done
```

For organization-specific checks, commit a policy file and pass it with `--schema-file`:

```json
{
  "required": [
    "x-ms-connector-metadata"
  ]
}
```
