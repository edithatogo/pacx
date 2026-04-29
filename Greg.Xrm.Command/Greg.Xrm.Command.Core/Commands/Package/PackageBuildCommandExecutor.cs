using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Package;

namespace Greg.Xrm.Command.Commands.Package
{
	public sealed class PackageBuildCommandExecutor(
		IOutput output,
		IPacxPackageBuilder packageBuilder
	) : ICommandExecutor<PackageBuildCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PackageBuildCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await packageBuilder.BuildAsync(command.Path, command.OutputPath, cancellationToken).ConfigureAwait(false);
				output.WriteLine($"Built PACX package: {result}", ConsoleColor.Green);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Unable to build package from <{command.Path}>: {ex.Message}", ex);
			}
		}
	}
}
