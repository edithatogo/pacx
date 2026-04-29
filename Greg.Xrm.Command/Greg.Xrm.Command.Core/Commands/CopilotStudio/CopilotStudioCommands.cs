using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.CopilotStudio
{
	public abstract class CopilotEnvironmentCommand
	{
		[Option("env", "e", Order = 1, Required = true, HelpText = "Power Platform environment ID.")]
		public string EnvironmentId { get; set; } = "";
	}

	[Command("copilot", "agent", "list", HelpText = "List Copilot Studio agents.")]
	public class CopilotAgentListCommand : CopilotEnvironmentCommand
	{
	}

	[Command("copilot", "agent", "create", HelpText = "Create a Copilot Studio agent.")]
	public class CopilotAgentCreateCommand : CopilotEnvironmentCommand
	{
		[Option("name", "n", Order = 2, Required = true, HelpText = "Agent display name.")]
		public string Name { get; set; } = "";

		[Option("solution", "s", Order = 3, HelpText = "Solution ID or unique name.")]
		public string? Solution { get; set; }
	}

	[Command("copilot", "agent", "publish", HelpText = "Publish a Copilot Studio agent.")]
	public class CopilotAgentPublishCommand : CopilotEnvironmentCommand
	{
		[Option("id", "i", Order = 2, Required = true, HelpText = "Agent ID.")]
		public string AgentId { get; set; } = "";
	}

	[Command("copilot", "agent", "clone", HelpText = "Clone a Copilot Studio agent to another environment.")]
	public class CopilotAgentCloneCommand : CopilotEnvironmentCommand
	{
		[Option("source-id", "s", Order = 2, Required = true, HelpText = "Source agent ID.")]
		public string SourceId { get; set; } = "";

		[Option("target-env", "t", Order = 3, Required = true, HelpText = "Target environment ID.")]
		public string TargetEnvironmentId { get; set; } = "";
	}

	[Command("copilot", "topic", "list", HelpText = "List Copilot Studio topics.")]
	public class CopilotTopicListCommand : CopilotEnvironmentCommand
	{
		[Option("agent-id", "a", Order = 2, Required = true, HelpText = "Agent ID.")]
		public string AgentId { get; set; } = "";
	}

	[Command("copilot", "topic", "export", HelpText = "Export Copilot Studio topics.")]
	public class CopilotTopicExportCommand : CopilotTopicListCommand
	{
		[Option("format", "f", Order = 3, DefaultValue = "json", HelpText = "Export format: json or yaml.")]
		public string Format { get; set; } = "json";
	}

	[Command("copilot", "topic", "import", HelpText = "Import Copilot Studio topics.")]
	public class CopilotTopicImportCommand : CopilotTopicListCommand
	{
		[Option("file", "f", Order = 3, Required = true, HelpText = "Topic export file.")]
		public string FilePath { get; set; } = "";
	}

	[Command("copilot", "knowledge", "list", HelpText = "List Copilot Studio knowledge sources.")]
	public class CopilotKnowledgeListCommand : CopilotTopicListCommand
	{
	}

	[Command("copilot", "knowledge", "add", HelpText = "Add a Copilot Studio knowledge source.")]
	public class CopilotKnowledgeAddCommand : CopilotTopicListCommand
	{
		[Option("source", "s", Order = 3, Required = true, HelpText = "Knowledge source URL, SharePoint path, or Dataverse identifier.")]
		public string Source { get; set; } = "";
	}

	[Command("copilot", "analytics", "sessions", HelpText = "Get Copilot Studio session analytics.")]
	public class CopilotAnalyticsSessionsCommand : CopilotTopicListCommand
	{
		[Option("days", "d", Order = 3, DefaultValue = 30, HelpText = "Number of days.")]
		public int Days { get; set; } = 30;
	}

	[Command("copilot", "analytics", "intents", HelpText = "Get Copilot Studio intent analytics.")]
	public class CopilotAnalyticsIntentsCommand : CopilotTopicListCommand
	{
	}

	[Command("copilot", "mcp", "expose", HelpText = "Describe an agent MCP tool exposure.")]
	public class CopilotMcpExposeCommand : CopilotTopicListCommand
	{
		[Option("tool-name", "n", Order = 3, HelpText = "MCP tool name.")]
		public string? ToolName { get; set; }
	}
}
