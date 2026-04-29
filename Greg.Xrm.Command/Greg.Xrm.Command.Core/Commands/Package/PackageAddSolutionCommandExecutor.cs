using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Package;

namespace Greg.Xrm.Command.Commands.Package
{
	public sealed class PackageAddSolutionCommandExecutor(
		IOutput output,
		IPacxPackageAuthoringService authoringService
	) : ICommandExecutor<PackageAddSolutionCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PackageAddSolutionCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await authoringService.AddSolutionAsync(command, cancellationToken).ConfigureAwait(false);
				output.WriteLine($"Added solution payload to package: {result}", ConsoleColor.Green);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Unable to add solution payload to <{command.Path}>: {ex.Message}", ex);
			}
		}
	}
}
