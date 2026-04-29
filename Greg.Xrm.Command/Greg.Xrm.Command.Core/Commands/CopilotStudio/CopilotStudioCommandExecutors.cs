using System.Text.Json;
using Greg.Xrm.Command.Services.CopilotStudio;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.CopilotStudio
{
	internal static class CopilotOutput
	{
		public static CommandResult WriteJson(IOutput output, JsonDocument document)
		{
			using (document)
			{
				output.WriteLine(JsonSerializer.Serialize(document.RootElement, new JsonSerializerOptions { WriteIndented = true }));
			}

			return CommandResult.Success();
		}
	}

	public class CopilotAgentListCommandExecutor(ICopilotStudioClient client, IOutput output) : ICommandExecutor<CopilotAgentListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(CopilotAgentListCommand command, CancellationToken cancellationToken)
			=> CopilotOutput.WriteJson(output, await client.GetAsync(command.EnvironmentId, "/agents", cancellationToken).ConfigureAwait(false));
	}

	public class CopilotAgentCreateCommandExecutor(ICopilotStudioClient client, IOutput output) : ICommandExecutor<CopilotAgentCreateCommand>
	{
		public async Task<CommandResult> ExecuteAsync(CopilotAgentCreateCommand command, CancellationToken cancellationToken)
			=> CopilotOutput.WriteJson(output, await client.PostAsync(command.EnvironmentId, "/agents", new { displayName = command.Name, solution = command.Solution }, cancellationToken).ConfigureAwait(false));
	}

	public class CopilotAgentPublishCommandExecutor(ICopilotStudioClient client, IOutput output) : ICommandExecutor<CopilotAgentPublishCommand>
	{
		public async Task<CommandResult> ExecuteAsync(CopilotAgentPublishCommand command, CancellationToken cancellationToken)
			=> CopilotOutput.WriteJson(output, await client.PostAsync(command.EnvironmentId, $"/agents/{CopilotCommandPaths.Escape(command.AgentId)}/publish", new { }, cancellationToken).ConfigureAwait(false));
	}

	public class CopilotAgentCloneCommandExecutor(ICopilotStudioClient client, IOutput output) : ICommandExecutor<CopilotAgentCloneCommand>
	{
		public async Task<CommandResult> ExecuteAsync(CopilotAgentCloneCommand command, CancellationToken cancellationToken)
			=> CopilotOutput.WriteJson(output, await client.PostAsync(command.EnvironmentId, $"/agents/{CopilotCommandPaths.Escape(command.SourceId)}/clone", new { targetEnvironmentId = command.TargetEnvironmentId }, cancellationToken).ConfigureAwait(false));
	}

	public class CopilotTopicListCommandExecutor(ICopilotStudioClient client, IOutput output) : ICommandExecutor<CopilotTopicListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(CopilotTopicListCommand command, CancellationToken cancellationToken)
			=> CopilotOutput.WriteJson(output, await client.GetAsync(command.EnvironmentId, $"/agents/{CopilotCommandPaths.Escape(command.AgentId)}/topics", cancellationToken).ConfigureAwait(false));
	}

	public class CopilotTopicExportCommandExecutor(ICopilotStudioClient client, IOutput output) : ICommandExecutor<CopilotTopicExportCommand>
	{
		public async Task<CommandResult> ExecuteAsync(CopilotTopicExportCommand command, CancellationToken cancellationToken)
			=> CopilotOutput.WriteJson(output, await client.GetAsync(command.EnvironmentId, $"/agents/{CopilotCommandPaths.Escape(command.AgentId)}/topics/export?format={CopilotCommandPaths.Escape(command.Format)}", cancellationToken).ConfigureAwait(false));
	}

	public class CopilotTopicImportCommandExecutor(ICopilotStudioClient client, IOutput output) : ICommandExecutor<CopilotTopicImportCommand>
	{
		public async Task<CommandResult> ExecuteAsync(CopilotTopicImportCommand command, CancellationToken cancellationToken)
		{
			var content = await File.ReadAllTextAsync(command.FilePath, cancellationToken).ConfigureAwait(false);
			return CopilotOutput.WriteJson(output, await client.PostAsync(command.EnvironmentId, $"/agents/{CopilotCommandPaths.Escape(command.AgentId)}/topics/import", new { content }, cancellationToken).ConfigureAwait(false));
		}
	}

	public class CopilotKnowledgeListCommandExecutor(ICopilotStudioClient client, IOutput output) : ICommandExecutor<CopilotKnowledgeListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(CopilotKnowledgeListCommand command, CancellationToken cancellationToken)
			=> CopilotOutput.WriteJson(output, await client.GetAsync(command.EnvironmentId, $"/agents/{CopilotCommandPaths.Escape(command.AgentId)}/knowledge", cancellationToken).ConfigureAwait(false));
	}

	public class CopilotKnowledgeAddCommandExecutor(ICopilotStudioClient client, IOutput output) : ICommandExecutor<CopilotKnowledgeAddCommand>
	{
		public async Task<CommandResult> ExecuteAsync(CopilotKnowledgeAddCommand command, CancellationToken cancellationToken)
			=> CopilotOutput.WriteJson(output, await client.PostAsync(command.EnvironmentId, $"/agents/{CopilotCommandPaths.Escape(command.AgentId)}/knowledge", new { source = command.Source }, cancellationToken).ConfigureAwait(false));
	}

	public class CopilotAnalyticsSessionsCommandExecutor(ICopilotStudioClient client, IOutput output) : ICommandExecutor<CopilotAnalyticsSessionsCommand>
	{
		public async Task<CommandResult> ExecuteAsync(CopilotAnalyticsSessionsCommand command, CancellationToken cancellationToken)
			=> CopilotOutput.WriteJson(output, await client.GetAsync(command.EnvironmentId, $"/agents/{CopilotCommandPaths.Escape(command.AgentId)}/analytics/sessions?days={command.Days}", cancellationToken).ConfigureAwait(false));
	}

	public class CopilotAnalyticsIntentsCommandExecutor(ICopilotStudioClient client, IOutput output) : ICommandExecutor<CopilotAnalyticsIntentsCommand>
	{
		public async Task<CommandResult> ExecuteAsync(CopilotAnalyticsIntentsCommand command, CancellationToken cancellationToken)
			=> CopilotOutput.WriteJson(output, await client.GetAsync(command.EnvironmentId, $"/agents/{CopilotCommandPaths.Escape(command.AgentId)}/analytics/intents", cancellationToken).ConfigureAwait(false));
	}

	public class CopilotMcpExposeCommandExecutor(IOutput output) : ICommandExecutor<CopilotMcpExposeCommand>
	{
		public Task<CommandResult> ExecuteAsync(CopilotMcpExposeCommand command, CancellationToken cancellationToken)
		{
			var toolName = command.ToolName ?? "copilot_" + command.AgentId.Replace('-', '_');
			output.WriteLine(JsonSerializer.Serialize(new { toolName, command.EnvironmentId, command.AgentId }, new JsonSerializerOptions { WriteIndented = true }));
			return Task.FromResult(CommandResult.Success());
		}
	}

	internal static class CopilotCommandPaths
	{
		public static string Escape(string value) => Uri.EscapeDataString(value);
	}
}
