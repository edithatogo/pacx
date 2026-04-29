using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Package;

namespace Greg.Xrm.Command.Commands.Package
{
	public sealed class PackageInitCommandExecutor(
		IOutput output,
		IPacxPackageInitializer packageInitializer
	) : ICommandExecutor<PackageInitCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PackageInitCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await packageInitializer.InitializeAsync(command, cancellationToken).ConfigureAwait(false);
				output.WriteLine($"Initialized PACX package at {result}", ConsoleColor.Green);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Unable to initialize package at <{command.Path}>: {ex.Message}", ex);
			}
		}
	}
}
