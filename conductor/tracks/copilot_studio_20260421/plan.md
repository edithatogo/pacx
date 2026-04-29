# Implementation Plan: Copilot Studio CLI

## Context
Zero Copilot Studio coverage. Only AI entry is the MCP server. Copilot Studio is increasingly the authoring surface for conversational AI on the Power Platform; lifecycle CLI (create/publish/export topics) is a natural fit.

## Phase 1: Auth & Client
- [x] Task: `ICopilotStudioClient` — wraps Copilot Studio REST / PVA API.
- [x] Task: Scope configured in `ITokenProvider`.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 2: Agent (Bot) Lifecycle
- [x] Task: `copilot agent list --env`.
- [x] Task: `copilot agent create --name --solution <id>`.
- [x] Task: `copilot agent publish --id`.
- [x] Task: `copilot agent clone --source-id --target-env`.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 3: Topic Management
- [x] Task: `copilot topic list --agent-id`.
- [x] Task: `copilot topic export --agent-id --format yaml|json` — YAML schema matches Copilot Studio authoring export.
- [x] Task: `copilot topic import --agent-id --file`.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 4: Generative Answers / Knowledge
- [x] Task: `copilot knowledge add --agent-id --source <url|sharepoint|dataverse>`.
- [x] Task: `copilot knowledge list --agent-id`.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 5: Analytics
- [x] Task: `copilot analytics sessions --agent-id --days 30`.
- [x] Task: `copilot analytics intents --agent-id` — top intents, escalation rate.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 6: MCP Bridge
- [x] Task: Expose selected agents as MCP metadata so Claude / Copilot-capable clients can invoke them through pacx's existing MCP server once the archived MCP permission blocker is resolved.
- [x] Task: Review pass completed locally; moved to the next phase.

## Phase 7: PR Lifecycle
- [x] Task: Working-tree implementation completed for upstream PR packaging.

## Validation
- Static JSON/config validation passed.
- Local build/test execution is blocked until the .NET 11 preview SDK is installed under the dotnet root used by `Greg.Xrm.Command/dotnet11.ps1`.
