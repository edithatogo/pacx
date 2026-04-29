using System.Text.Json;
using Greg.Xrm.Command.Services.DesktopFlows;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.DesktopFlows
{
	internal static class DesktopFlowCommandOutput
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

	public class DesktopFlowListCommandExecutor(IDesktopFlowClient client, IOutput output) : ICommandExecutor<DesktopFlowListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(DesktopFlowListCommand command, CancellationToken cancellationToken)
			=> DesktopFlowCommandOutput.WriteJson(output, await client.ListDesktopFlowsAsync(command.EnvironmentId, cancellationToken).ConfigureAwait(false));
	}

	public class DesktopFlowGetCommandExecutor(IDesktopFlowClient client, IOutput output) : ICommandExecutor<DesktopFlowGetCommand>
	{
		public async Task<CommandResult> ExecuteAsync(DesktopFlowGetCommand command, CancellationToken cancellationToken)
			=> DesktopFlowCommandOutput.WriteJson(output, await client.GetDesktopFlowAsync(command.Id, cancellationToken).ConfigureAwait(false));
	}

	public class DesktopFlowTriggerCommandExecutor(IDesktopFlowClient client, IOutput output) : ICommandExecutor<DesktopFlowTriggerCommand>
	{
		public async Task<CommandResult> ExecuteAsync(DesktopFlowTriggerCommand command, CancellationToken cancellationToken)
			=> DesktopFlowCommandOutput.WriteJson(output, await client.TriggerDesktopFlowAsync(command.EnvironmentId, command.Id, command.MachineGroup, ParseInputs(command.Input), cancellationToken).ConfigureAwait(false));

		internal static IReadOnlyDictionary<string, string> ParseInputs(string? value)
		{
			var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			if (string.IsNullOrWhiteSpace(value))
			{
				return result;
			}

			foreach (var item in value.Split('\x1F', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
			{
				var index = item.IndexOf('=', StringComparison.Ordinal);
				if (index <= 0)
				{
					continue;
				}

				result[item[..index]] = item[(index + 1)..];
			}

			return result;
		}
	}

	public class DesktopFlowRunListCommandExecutor(IDesktopFlowClient client, IOutput output) : ICommandExecutor<DesktopFlowRunListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(DesktopFlowRunListCommand command, CancellationToken cancellationToken)
			=> DesktopFlowCommandOutput.WriteJson(output, await client.ListRunsAsync(command.EnvironmentId, command.Id, cancellationToken).ConfigureAwait(false));
	}

	public class DesktopFlowRunGetCommandExecutor(IDesktopFlowClient client, IOutput output) : ICommandExecutor<DesktopFlowRunGetCommand>
	{
		public async Task<CommandResult> ExecuteAsync(DesktopFlowRunGetCommand command, CancellationToken cancellationToken)
			=> DesktopFlowCommandOutput.WriteJson(output, await client.GetRunAsync(command.EnvironmentId, command.RunId, cancellationToken).ConfigureAwait(false));
	}

	public class DesktopFlowMachineListCommandExecutor(IDesktopFlowClient client, IOutput output) : ICommandExecutor<DesktopFlowMachineListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(DesktopFlowMachineListCommand command, CancellationToken cancellationToken)
			=> DesktopFlowCommandOutput.WriteJson(output, await client.ListMachinesAsync(command.EnvironmentId, cancellationToken).ConfigureAwait(false));
	}

	public class DesktopFlowMachineGroupListCommandExecutor(IDesktopFlowClient client, IOutput output) : ICommandExecutor<DesktopFlowMachineGroupListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(DesktopFlowMachineGroupListCommand command, CancellationToken cancellationToken)
			=> DesktopFlowCommandOutput.WriteJson(output, await client.ListMachineGroupsAsync(command.EnvironmentId, cancellationToken).ConfigureAwait(false));
	}

	public class DesktopFlowMachineGroupAssignCommandExecutor(IDesktopFlowClient client, IOutput output) : ICommandExecutor<DesktopFlowMachineGroupAssignCommand>
	{
		public async Task<CommandResult> ExecuteAsync(DesktopFlowMachineGroupAssignCommand command, CancellationToken cancellationToken)
			=> DesktopFlowCommandOutput.WriteJson(output, await client.AssignMachineToGroupAsync(command.EnvironmentId, command.MachineId, command.GroupId, cancellationToken).ConfigureAwait(false));
	}

	public class DesktopFlowScaffoldCommandExecutor(IOutput output) : ICommandExecutor<DesktopFlowScaffoldCommand>
	{
		public async Task<CommandResult> ExecuteAsync(DesktopFlowScaffoldCommand command, CancellationToken cancellationToken)
		{
			var path = command.OutputPath ?? command.Name.Replace(' ', '_') + ".txt";
			var script = $"""
			# {command.Name}
			# Power Automate Desktop script skeleton

			Variables.CreateNewList List=> InputRows
			Display.ShowMessageDialog.ShowMessage Title: '{command.Name}' Message: 'Desktop flow scaffold created by pacx.'
			""";

			await File.WriteAllTextAsync(path, script, cancellationToken).ConfigureAwait(false);
			output.WriteLine($"Desktop flow scaffold written to: {path}", ConsoleColor.Green);
			return CommandResult.Success();
		}
	}

	public class ApprovalListCommandExecutor(IDesktopFlowClient client, IOutput output) : ICommandExecutor<ApprovalListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(ApprovalListCommand command, CancellationToken cancellationToken)
			=> DesktopFlowCommandOutput.WriteJson(output, await client.ListApprovalsAsync(command.EnvironmentId, cancellationToken).ConfigureAwait(false));
	}

	public class ApprovalRespondCommandExecutor(IDesktopFlowClient client, IOutput output) : ICommandExecutor<ApprovalRespondCommand>
	{
		public async Task<CommandResult> ExecuteAsync(ApprovalRespondCommand command, CancellationToken cancellationToken)
			=> DesktopFlowCommandOutput.WriteJson(output, await client.RespondToApprovalAsync(command.EnvironmentId, command.ApprovalId, command.Decision, command.Comment, cancellationToken).ConfigureAwait(false));
	}
}
