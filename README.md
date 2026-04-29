# PacPacx

[![CI](https://github.com/edithatogo/Greg.Xrm.Command/actions/workflows/ci.yml/badge.svg)](https://github.com/edithatogo/Greg.Xrm.Command/actions/workflows/ci.yml)
[![Docs](https://github.com/edithatogo/Greg.Xrm.Command/actions/workflows/docs.yml/badge.svg)](https://github.com/edithatogo/Greg.Xrm.Command/actions/workflows/docs.yml)
[![Coverage](https://codecov.io/gh/edithatogo/Greg.Xrm.Command/branch/master/graph/badge.svg)](https://codecov.io/gh/edithatogo/Greg.Xrm.Command)
[![NuGet](https://img.shields.io/nuget/v/Greg.Xrm.Command.svg)](https://www.nuget.org/packages/Greg.Xrm.Command)
[![OpenSSF Scorecard](https://github.com/edithatogo/Greg.Xrm.Command/actions/workflows/scorecard.yml/badge.svg)](https://github.com/edithatogo/Greg.Xrm.Command/actions/workflows/scorecard.yml)
![OpenSSF Best Practices](https://img.shields.io/badge/OpenSSF%20Best%20Practices-not%20configured-lightgrey)

PacPacx is a Power Platform automation workspace for the `pacx` CLI, focused on Dataverse, ALM, AI Builder, custom connectors, MCP hosting, and repository workflows. It extends the Microsoft `pac` style of command-line automation with a broader command surface and track-based implementation plans.

## Documentation

The documentation site source lives in [docs/](docs/). Start with:

- [Getting started](docs/index.md)
- [Authentication](docs/guides/authentication.md)
- [Generated command reference](docs/reference/commands/generated/index.md)
- [Recipes](docs/recipes/toc.yml)
- [Migration from pac](docs/guides/migration-from-pac.md)
- [Architecture decisions](docs/adr/index.md)

The generated command reference is refreshed with:

```powershell
.\scripts\generate-command-docs.ps1
```

| Reference coverage | Count |
| --- | ---: |
| Generated command pages | 189 |
| Top-level command areas | 50 |

Largest command areas: `solution`, `column`, `package`, `plugin`, `workflow`, `webresources`, `view`, `auth`, `rel`, `table`, `settings`, and `completions`.

## Build

```powershell
dotnet restore
dotnet build
dotnet test
```

The solution is pinned to the published .NET 11 preview SDK line through `global.json`. If `dotnet` resolves the wrong SDK, clear stale `MSBuildSDKsPath` overrides and use a user-profile SDK install when admin access is unavailable.

## Development

- Open the repo in the devcontainer under [.devcontainer/](.devcontainer/) for a preconfigured toolchain.
- Run `dotnet husky install` once after cloning to enable local hooks.
- Keep `trufflehog` and `commitlint` on `PATH` for the same checks enforced by CI.
- Use the templates under [.github/](.github/) for issues, pull requests, funding metadata, and security reporting.

## Project Files

- [PAC_PACX_INVENTORY.md](PAC_PACX_INVENTORY.md) lists PAC and PACX command inventory details.
- [POWER_PLATFORM_MCP_SETUP.md](POWER_PLATFORM_MCP_SETUP.md) documents MCP server configuration.
- [tracks/](tracks/) and [conductor/tracks/](conductor/tracks/) hold declarative implementation plans and conductor status.

## Security

Do not commit secrets. Use vault-backed credentials for production automation, follow least privilege for Azure RBAC, and include security impact notes in pull requests that change authentication, connector, deployment, or MCP boundaries.

## License

TBD. Add a `LICENSE` file before publishing packages or releases.
