using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Package;

namespace Greg.Xrm.Command.Commands.Package
{
	public sealed class PackageReleaseVerifyCommandExecutor(
		IOutput output,
		IPacxPackageReleaseVerifier releaseVerifier
	) : ICommandExecutor<PackageReleaseVerifyCommand>
	{
		public Task<CommandResult> ExecuteAsync(PackageReleaseVerifyCommand command, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var result = releaseVerifier.Verify(command.Path);

			if (result.Exception is not null)
			{
				return Task.FromResult(CommandResult.Fail($"Release verification failed for <{command.Path}>: {result.Exception.Message}", result.Exception));
			}

			if (result.IsValid)
			{
				output.WriteLine("PACX release verification passed.", ConsoleColor.Green);
				return Task.FromResult(CommandResult.Success());
			}

			if (result.MissingFiles.Count > 0)
			{
				output.WriteLine("Missing release files:", ConsoleColor.Red);
				foreach (var missing in result.MissingFiles)
				{
					output.WriteLine($"- {missing}", ConsoleColor.Red);
				}
			}

			if (result.ChecksumErrors.Count > 0)
			{
				output.WriteLine("Release integrity issues:", ConsoleColor.Red);
				foreach (var issue in result.ChecksumErrors)
				{
					output.WriteLine($"- {issue}", ConsoleColor.Red);
				}
			}

			return Task.FromResult(CommandResult.Fail(
				$"Release verification failed for <{command.Path}>: {result.MissingFiles.Count} missing file(s), {result.ChecksumErrors.Count} integrity issue(s)."));
		}
	}
}
