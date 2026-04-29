using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Package;

namespace Greg.Xrm.Command.Commands.Package
{
	public sealed class PackageRemoveSolutionCommandExecutor(
		IOutput output,
		IPacxPackageAuthoringService authoringService
	) : ICommandExecutor<PackageRemoveSolutionCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PackageRemoveSolutionCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await authoringService.RemoveSolutionAsync(command, cancellationToken).ConfigureAwait(false);
				output.WriteLine($"Removed solution payload from package: {result}", ConsoleColor.Green);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Unable to remove solution payload from <{command.Path}>: {ex.Message}", ex);
			}
		}
	}
}
