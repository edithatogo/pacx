using System.Text.Json;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.PowerBi;

namespace Greg.Xrm.Command.Commands.PowerBi
{
	internal static class PowerBiCommandOutput
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

	public class DatasetListCommandExecutor(IPowerBiClient client, IOutput output) : ICommandExecutor<DatasetListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(DatasetListCommand command, CancellationToken cancellationToken)
			=> PowerBiCommandOutput.WriteJson(output, await client.GetAsync($"/groups/{PowerBiCommandPaths.Escape(command.WorkspaceId)}/datasets", cancellationToken).ConfigureAwait(false));
	}

	public class DatasetPublishCommandExecutor(IPowerBiClient client, IOutput output) : ICommandExecutor<DatasetPublishCommand>
	{
		public async Task<CommandResult> ExecuteAsync(DatasetPublishCommand command, CancellationToken cancellationToken)
		{
			if (!File.Exists(command.PbixPath))
			{
				return CommandResult.Fail($"PBIX file not found: {command.PbixPath}");
			}

			var name = Uri.EscapeDataString(command.Name ?? Path.GetFileNameWithoutExtension(command.PbixPath));
			return PowerBiCommandOutput.WriteJson(output, await client.PostFileAsync($"/groups/{PowerBiCommandPaths.Escape(command.WorkspaceId)}/imports?datasetDisplayName={name}", command.PbixPath, cancellationToken).ConfigureAwait(false));
		}
	}

	public class DatasetCloneCommandExecutor(IPowerBiClient client, IOutput output) : ICommandExecutor<DatasetCloneCommand>
	{
		public async Task<CommandResult> ExecuteAsync(DatasetCloneCommand command, CancellationToken cancellationToken)
			=> PowerBiCommandOutput.WriteJson(output, await client.PostAsync($"/datasets/{PowerBiCommandPaths.Escape(command.SourceId)}/Clone", new { targetWorkspaceId = command.TargetWorkspaceId }, cancellationToken).ConfigureAwait(false));
	}

	public class DatasetDeleteCommandExecutor(IPowerBiClient client, IOutput output) : ICommandExecutor<DatasetDeleteCommand>
	{
		public async Task<CommandResult> ExecuteAsync(DatasetDeleteCommand command, CancellationToken cancellationToken)
			=> PowerBiCommandOutput.WriteJson(output, await client.DeleteAsync($"/groups/{PowerBiCommandPaths.Escape(command.WorkspaceId)}/datasets/{PowerBiCommandPaths.Escape(command.DatasetId)}", cancellationToken).ConfigureAwait(false));
	}

	public class DatasetRlsListCommandExecutor(IPowerBiClient client, IOutput output) : ICommandExecutor<DatasetRlsListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(DatasetRlsListCommand command, CancellationToken cancellationToken)
			=> PowerBiCommandOutput.WriteJson(output, await client.GetAsync($"/groups/{PowerBiCommandPaths.Escape(command.WorkspaceId)}/datasets/{PowerBiCommandPaths.Escape(command.DatasetId)}/users", cancellationToken).ConfigureAwait(false));
	}

	public class DatasetRlsApplyCommandExecutor(IPowerBiClient client, IOutput output) : ICommandExecutor<DatasetRlsApplyCommand>
	{
		public async Task<CommandResult> ExecuteAsync(DatasetRlsApplyCommand command, CancellationToken cancellationToken)
			=> PowerBiCommandOutput.WriteJson(output, await client.PostAsync($"/groups/{PowerBiCommandPaths.Escape(command.WorkspaceId)}/datasets/{PowerBiCommandPaths.Escape(command.DatasetId)}/users", new { identifier = command.User, principalType = "User", datasetUserAccessRight = "Read", roles = new[] { command.Role } }, cancellationToken).ConfigureAwait(false));
	}

	public class DatasetRefreshTriggerCommandExecutor(IPowerBiClient client, IOutput output) : ICommandExecutor<DatasetRefreshTriggerCommand>
	{
		public async Task<CommandResult> ExecuteAsync(DatasetRefreshTriggerCommand command, CancellationToken cancellationToken)
			=> PowerBiCommandOutput.WriteJson(output, await client.PostAsync(PowerBiCommandPaths.DatasetRefreshPath(command), new { notifyOption = "NoNotification" }, cancellationToken).ConfigureAwait(false));
	}

	public class DatasetRefreshStatusCommandExecutor(IPowerBiClient client, IOutput output) : ICommandExecutor<DatasetRefreshStatusCommand>
	{
		public async Task<CommandResult> ExecuteAsync(DatasetRefreshStatusCommand command, CancellationToken cancellationToken)
			=> PowerBiCommandOutput.WriteJson(output, await client.GetAsync(PowerBiCommandPaths.DatasetRefreshPath(command), cancellationToken).ConfigureAwait(false));
	}

	public class DatasetRefreshScheduleCommandExecutor(IPowerBiClient client, IOutput output) : ICommandExecutor<DatasetRefreshScheduleCommand>
	{
		public async Task<CommandResult> ExecuteAsync(DatasetRefreshScheduleCommand command, CancellationToken cancellationToken)
			=> PowerBiCommandOutput.WriteJson(output, await client.PostAsync($"/groups/{PowerBiCommandPaths.Escape(command.WorkspaceId)}/datasets/{PowerBiCommandPaths.Escape(command.DatasetId)}/refreshSchedule", new { enabled = true, localTimeZoneId = "UTC", cron = command.Cron }, cancellationToken).ConfigureAwait(false));
	}

	public class PipelineListCommandExecutor(IPowerBiClient client, IOutput output) : ICommandExecutor<PipelineListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PipelineListCommand command, CancellationToken cancellationToken)
			=> PowerBiCommandOutput.WriteJson(output, await client.GetAsync("/pipelines", cancellationToken).ConfigureAwait(false));
	}

	public class PipelineStageAssignCommandExecutor(IPowerBiClient client, IOutput output) : ICommandExecutor<PipelineStageAssignCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PipelineStageAssignCommand command, CancellationToken cancellationToken)
			=> PowerBiCommandOutput.WriteJson(output, await client.PostAsync($"/pipelines/{PowerBiCommandPaths.Escape(command.PipelineId)}/stages/{PowerBiCommandPaths.Escape(command.Stage)}/assignWorkspace", new { workspaceId = command.WorkspaceId }, cancellationToken).ConfigureAwait(false));
	}

	public class PipelineDeployCommandExecutor(IPowerBiClient client, IOutput output) : ICommandExecutor<PipelineDeployCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PipelineDeployCommand command, CancellationToken cancellationToken)
			=> PowerBiCommandOutput.WriteJson(output, await client.PostAsync($"/pipelines/{PowerBiCommandPaths.Escape(command.PipelineId)}/deployAll", new { sourceStageOrder = command.SourceStage, targetStageOrder = command.TargetStage }, cancellationToken).ConfigureAwait(false));
	}

	public class CapacityListCommandExecutor(IPowerBiClient client, IOutput output) : ICommandExecutor<CapacityListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(CapacityListCommand command, CancellationToken cancellationToken)
			=> PowerBiCommandOutput.WriteJson(output, await client.GetAsync("/capacities", cancellationToken).ConfigureAwait(false));
	}

	public class CapacityWorkspaceAssignCommandExecutor(IPowerBiClient client, IOutput output) : ICommandExecutor<CapacityWorkspaceAssignCommand>
	{
		public async Task<CommandResult> ExecuteAsync(CapacityWorkspaceAssignCommand command, CancellationToken cancellationToken)
			=> PowerBiCommandOutput.WriteJson(output, await client.PostAsync($"/groups/{PowerBiCommandPaths.Escape(command.WorkspaceId)}/AssignToCapacity", new { capacityId = command.CapacityId }, cancellationToken).ConfigureAwait(false));
	}

	public class CapacityMetricsCommandExecutor(IPowerBiClient client, IOutput output) : ICommandExecutor<CapacityMetricsCommand>
	{
		public async Task<CommandResult> ExecuteAsync(CapacityMetricsCommand command, CancellationToken cancellationToken)
			=> PowerBiCommandOutput.WriteJson(output, await client.GetAsync($"/capacities/{PowerBiCommandPaths.Escape(command.CapacityId)}/workloads", cancellationToken).ConfigureAwait(false));
	}

	internal static class PowerBiCommandPaths
	{
		public static string DatasetRefreshPath(DatasetCommandBase command)
			=> $"/groups/{Escape(command.WorkspaceId)}/datasets/{Escape(command.DatasetId)}/refreshes";

		public static string Escape(string value) => Uri.EscapeDataString(value);
	}
}
