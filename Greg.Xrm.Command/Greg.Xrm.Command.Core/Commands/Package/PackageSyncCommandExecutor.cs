using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Package;

namespace Greg.Xrm.Command.Commands.Package
{
	public sealed class PackageSyncCommandExecutor(
		IOutput output,
		IPacxPackageAuthoringService authoringService
	) : ICommandExecutor<PackageSyncCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PackageSyncCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await authoringService.SyncAsync(command, cancellationToken).ConfigureAwait(false);
				output.WriteLine(
					$"Synchronized package folder: +{result.AddedArtifacts} artifacts, +{result.AddedSteps} steps, -{result.PrunedArtifacts} artifacts, -{result.PrunedSteps} steps",
					ConsoleColor.Green);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Unable to synchronize package folder <{command.Path}>: {ex.Message}", ex);
			}
		}
	}
}
