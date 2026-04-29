using System.Text.Json;
using Greg.Xrm.Command.Services.Fabric;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Fabric
{
	internal static class FabricCommandOutput
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

	public class FabricWorkspaceListCommandExecutor(IFabricClient client, IOutput output) : ICommandExecutor<FabricWorkspaceListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FabricWorkspaceListCommand command, CancellationToken cancellationToken)
			=> FabricCommandOutput.WriteJson(output, await client.GetAsync("/workspaces", cancellationToken).ConfigureAwait(false));
	}

	public class FabricWorkspaceCreateCommandExecutor(IFabricClient client, IOutput output) : ICommandExecutor<FabricWorkspaceCreateCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FabricWorkspaceCreateCommand command, CancellationToken cancellationToken)
			=> FabricCommandOutput.WriteJson(output, await client.PostAsync("/workspaces", new { displayName = command.Name, capacityId = command.CapacityId }, cancellationToken).ConfigureAwait(false));
	}

	public class FabricCapacityListCommandExecutor(IFabricClient client, IOutput output) : ICommandExecutor<FabricCapacityListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FabricCapacityListCommand command, CancellationToken cancellationToken)
			=> FabricCommandOutput.WriteJson(output, await client.GetAsync("/capacities", cancellationToken).ConfigureAwait(false));
	}

	public class FabricLakehouseListCommandExecutor(IFabricClient client, IOutput output) : ICommandExecutor<FabricLakehouseListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FabricLakehouseListCommand command, CancellationToken cancellationToken)
			=> FabricCommandOutput.WriteJson(output, await client.GetAsync($"/workspaces/{Uri.EscapeDataString(command.WorkspaceId)}/lakehouses", cancellationToken).ConfigureAwait(false));
	}

	public class FabricLakehouseCreateCommandExecutor(IFabricClient client, IOutput output) : ICommandExecutor<FabricLakehouseCreateCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FabricLakehouseCreateCommand command, CancellationToken cancellationToken)
			=> FabricCommandOutput.WriteJson(output, await client.PostAsync($"/workspaces/{Uri.EscapeDataString(command.WorkspaceId)}/lakehouses", new { displayName = command.Name }, cancellationToken).ConfigureAwait(false));
	}

	public class FabricSemanticModelListCommandExecutor(IFabricClient client, IOutput output) : ICommandExecutor<FabricSemanticModelListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FabricSemanticModelListCommand command, CancellationToken cancellationToken)
			=> FabricCommandOutput.WriteJson(output, await client.GetAsync($"/workspaces/{Uri.EscapeDataString(command.WorkspaceId)}/semanticModels", cancellationToken).ConfigureAwait(false));
	}

	public class FabricSemanticModelRefreshCommandExecutor(IFabricClient client, IOutput output) : ICommandExecutor<FabricSemanticModelRefreshCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FabricSemanticModelRefreshCommand command, CancellationToken cancellationToken)
			=> FabricCommandOutput.WriteJson(output, await client.PostAsync($"/workspaces/{Uri.EscapeDataString(command.WorkspaceId)}/semanticModels/{Uri.EscapeDataString(command.SemanticModelId)}/refreshes", new { }, cancellationToken).ConfigureAwait(false));
	}

	public class FabricLinkCreateCommandExecutor(IFabricClient client, IOutput output) : ICommandExecutor<FabricLinkCreateCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FabricLinkCreateCommand command, CancellationToken cancellationToken)
			=> FabricCommandOutput.WriteJson(output, await client.PostAsync("/dataverseLinks", new { dataverseEnvironment = command.DataverseEnvironment, targetWorkspaceId = command.TargetWorkspaceId }, cancellationToken).ConfigureAwait(false));
	}

	public class FabricLinkStatusCommandExecutor(IFabricClient client, IOutput output) : ICommandExecutor<FabricLinkStatusCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FabricLinkStatusCommand command, CancellationToken cancellationToken)
			=> FabricCommandOutput.WriteJson(output, await client.GetAsync("/dataverseLinks", cancellationToken).ConfigureAwait(false));
	}
}
