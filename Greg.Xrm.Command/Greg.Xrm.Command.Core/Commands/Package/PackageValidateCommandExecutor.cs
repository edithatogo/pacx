using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Package;

namespace Greg.Xrm.Command.Commands.Package
{
	public sealed class PackageValidateCommandExecutor(
		IOutput output,
		IPacxPackageReader packageReader
	) : ICommandExecutor<PackageValidateCommand>
	{
		public Task<CommandResult> ExecuteAsync(PackageValidateCommand command, CancellationToken cancellationToken)
		{
			try
			{
				using var package = packageReader.Open(command.Path);

				output.WriteLine($"Valid PACX package: {package.Manifest.PackageId} {package.Manifest.Version}", ConsoleColor.Green);
				output.WriteLine($"  Kind: {PacxPackageKinds.Describe(package.Manifest.Kind)}");
				output.WriteLine($"  Contract: {PacxPackageKinds.DescribeContract(package.Manifest.Kind)}");
				output.WriteLine($"  Artifacts: {package.Entries.Count}");
				output.WriteLine($"  Deployment steps: {package.Manifest.Deployment.Count}");
				return Task.FromResult(CommandResult.Success());
			}
			catch (Exception ex)
			{
				return Task.FromResult(CommandResult.Fail($"Package validation failed for <{command.Path}>: {ex.Message}", ex));
			}
		}
	}
}
