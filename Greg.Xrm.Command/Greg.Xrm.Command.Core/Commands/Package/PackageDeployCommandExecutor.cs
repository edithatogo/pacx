using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Package;

namespace Greg.Xrm.Command.Commands.Package
{
	public sealed class PackageDeployCommandExecutor(
		IOutput output,
		IPacxPackageReader packageReader,
		IPacxPackageDeployer packageDeployer
	) : ICommandExecutor<PackageDeployCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PackageDeployCommand command, CancellationToken cancellationToken)
		{
			try
			{
				using var package = packageReader.Open(command.Path);
				output.WriteLine($"Loaded PACX package {package.Manifest.PackageId} {package.Manifest.Version}", ConsoleColor.Cyan);
				await packageDeployer.DeployAsync(package, command.DryRun, cancellationToken).ConfigureAwait(false);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Unable to deploy package <{command.Path}>: {ex.Message}", ex);
			}
		}
	}
}
