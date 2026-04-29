using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Package;

namespace Greg.Xrm.Command.Commands.Package
{
	public sealed class PackagePublishCommandExecutor(
		IOutput output,
		IPacxPackagePublisher packagePublisher
	) : ICommandExecutor<PackagePublishCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PackagePublishCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await packagePublisher.PublishAsync(command, cancellationToken).ConfigureAwait(false);
				output.WriteLine($"Published PACX archive: {result.PackagePath}", ConsoleColor.Green);
				output.WriteLine($"  Release manifest: {result.ReleaseManifestPath}");
				output.WriteLine($"  SHA256: {result.ArchiveSha256}");
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Unable to publish package <{command.Path}>: {ex.Message}", ex);
			}
		}
	}
}
