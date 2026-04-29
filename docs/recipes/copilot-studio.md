# Copilot Studio

Use the Copilot Studio commands to manage agents, topics, knowledge, analytics, and MCP exposure metadata.

```powershell
pacx copilot agent list --env <environment-id>
pacx copilot agent create --env <environment-id> --name "Support Agent" --solution <solution-id>
pacx copilot topic export --env <environment-id> --agent-id <agent-id> --format json
pacx copilot knowledge add --env <environment-id> --agent-id <agent-id> --source https://contoso/sites/support
pacx copilot analytics sessions --env <environment-id> --agent-id <agent-id> --days 30
pacx copilot mcp expose --env <environment-id> --agent-id <agent-id> --tool-name support_agent
```
