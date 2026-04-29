using Greg.Xrm.Command.Services.Fabric;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Fabric
{
	public class OneLakeShortcutListCommandExecutor(IFabricClient client, IOutput output) : ICommandExecutor<OneLakeShortcutListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(OneLakeShortcutListCommand command, CancellationToken cancellationToken)
			=> FabricCommandOutput.WriteJson(output, await client.GetAsync(BuildShortcutPath(command.WorkspaceId, command.ItemId), cancellationToken).ConfigureAwait(false));

		internal static string BuildShortcutPath(string workspaceId, string itemId)
			=> $"/workspaces/{Uri.EscapeDataString(workspaceId)}/items/{Uri.EscapeDataString(itemId)}/shortcuts";
	}

	public class OneLakeShortcutCreateCommandExecutor(IFabricClient client, IOutput output) : ICommandExecutor<OneLakeShortcutCreateCommand>
	{
		public async Task<CommandResult> ExecuteAsync(OneLakeShortcutCreateCommand command, CancellationToken cancellationToken)
		{
			var payload = new
			{
				path = command.TargetPath,
				target = new
				{
					type = command.SourceType,
					path = command.SourcePath
				}
			};

			return FabricCommandOutput.WriteJson(output, await client.PostAsync(OneLakeShortcutListCommandExecutor.BuildShortcutPath(command.WorkspaceId, command.ItemId), payload, cancellationToken).ConfigureAwait(false));
		}
	}

	public class OneLakeShortcutDeleteCommandExecutor(IFabricClient client, IOutput output) : ICommandExecutor<OneLakeShortcutDeleteCommand>
	{
		public async Task<CommandResult> ExecuteAsync(OneLakeShortcutDeleteCommand command, CancellationToken cancellationToken)
			=> FabricCommandOutput.WriteJson(output, await client.DeleteAsync(OneLakeShortcutListCommandExecutor.BuildShortcutPath(command.WorkspaceId, command.ItemId) + "/" + Uri.EscapeDataString(command.ShortcutPath), cancellationToken).ConfigureAwait(false));
	}
}
