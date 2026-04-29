using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Package;

namespace Greg.Xrm.Command.Commands.Package
{
	public sealed class PackageReleaseCommandExecutor(
		IOutput output,
		IPacxPackageReleaser packageReleaser
	) : ICommandExecutor<PackageReleaseCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PackageReleaseCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await packageReleaser.ReleaseAsync(command, cancellationToken).ConfigureAwait(false);
				output.WriteLine($"Staged PACX release: {result.ReleaseDirectory}", ConsoleColor.Green);
				output.WriteLine($"  Package: {result.PackagePath}");
				output.WriteLine($"  Manifest: {result.ReleaseManifestPath}");
				output.WriteLine($"  Provenance: {result.ProvenancePath}");
				output.WriteLine($"  SBOM: {result.SbomPath}");
				output.WriteLine($"  Notes: {result.ReleaseNotesPath}");
				output.WriteLine($"  Checksums: {result.ChecksumsPath}");
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Unable to stage release for <{command.Path}>: {ex.Message}", ex);
			}
		}
	}
}
