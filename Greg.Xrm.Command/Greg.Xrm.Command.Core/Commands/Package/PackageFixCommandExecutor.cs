using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Package;

namespace Greg.Xrm.Command.Commands.Package
{
	public sealed class PackageFixCommandExecutor(
		IOutput output,
		IPacxPackageAuthoringService authoringService
	) : ICommandExecutor<PackageFixCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PackageFixCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await authoringService.FixAsync(command, cancellationToken).ConfigureAwait(false);
				output.WriteLine(
					$"Fixed package folder: +{result.AddedArtifacts} artifacts, +{result.AddedSteps} steps, -{result.DedupedArtifacts} duplicate artifacts, -{result.DedupedSteps} duplicate steps, -{result.PrunedArtifacts} stale artifacts, -{result.PrunedSteps} stale steps",
					ConsoleColor.Green);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Unable to fix package folder <{command.Path}>: {ex.Message}", ex);
			}
		}
	}
}
