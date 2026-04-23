# Implementation Plan: Copilot Studio CLI

## Context
Zero Copilot Studio coverage. Only AI entry is the MCP server. Copilot Studio is increasingly the authoring surface for conversational AI on the Power Platform; lifecycle CLI (create/publish/export topics) is a natural fit.

## Phase 1: Auth & Client
- [ ] Task: `ICopilotStudioClient` — wraps Copilot Studio REST / PVA API.
- [ ] Task: Scope configured in `ITokenProvider`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 2: Agent (Bot) Lifecycle
- [ ] Task: `copilot agent list --env`.
- [ ] Task: `copilot agent create --name --solution <id>`.
- [ ] Task: `copilot agent publish --id`.
- [ ] Task: `copilot agent clone --source-id --target-env`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 3: Topic Management
- [ ] Task: `copilot topic list --agent-id`.
- [ ] Task: `copilot topic export --agent-id --format yaml|json` — YAML schema matches Copilot Studio authoring export.
- [ ] Task: `copilot topic import --agent-id --file`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 4: Generative Answers / Knowledge
- [ ] Task: `copilot knowledge add --agent-id --source <url|sharepoint|dataverse>`.
- [ ] Task: `copilot knowledge list --agent-id`.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 5: Analytics
- [ ] Task: `copilot analytics sessions --agent-id --days 30`.
- [ ] Task: `copilot analytics intents --agent-id` — top intents, escalation rate.
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 6: MCP Bridge
- [ ] Task: Expose selected agents as MCP tools so Claude / Copilot-capable clients can invoke them through pacx's existing MCP server (cross-pollinates with `mcp_server_20260408`).
- [ ] Task: Run /conductor:review, automatically apply fixes, and progress to the next phase.

## Phase 7: PR Lifecycle
- [ ] Task: Upstream PR; `/ralph-loop`; merge.
