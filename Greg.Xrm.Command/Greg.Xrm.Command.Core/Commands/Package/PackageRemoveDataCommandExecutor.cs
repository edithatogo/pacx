using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Package;

namespace Greg.Xrm.Command.Commands.Package
{
	public sealed class PackageRemoveDataCommandExecutor(
		IOutput output,
		IPacxPackageAuthoringService authoringService
	) : ICommandExecutor<PackageRemoveDataCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PackageRemoveDataCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await authoringService.RemoveDataAsync(command, cancellationToken).ConfigureAwait(false);
				output.WriteLine($"Removed data payload from package: {result}", ConsoleColor.Green);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Unable to remove data payload from <{command.Path}>: {ex.Message}", ex);
			}
		}
	}
}
