using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Package;

namespace Greg.Xrm.Command.Commands.Package
{
	public sealed class PackageAddDataCommandExecutor(
		IOutput output,
		IPacxPackageAuthoringService authoringService
	) : ICommandExecutor<PackageAddDataCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PackageAddDataCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await authoringService.AddDataAsync(command, cancellationToken).ConfigureAwait(false);
				output.WriteLine($"Added data payload to package: {result}", ConsoleColor.Green);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Unable to add data payload to <{command.Path}>: {ex.Message}", ex);
			}
		}
	}
}
