using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Package;

namespace Greg.Xrm.Command.Commands.Package
{
	public sealed class PackageShowCommandExecutor(
		IOutput output,
		IPacxPackageReader packageReader
	) : ICommandExecutor<PackageShowCommand>
	{
		public Task<CommandResult> ExecuteAsync(PackageShowCommand command, CancellationToken cancellationToken)
		{
			try
			{
				using var package = packageReader.Open(command.Path);

				output.WriteLine($"PACX package: {package.Manifest.Name}", ConsoleColor.Cyan);
				output.WriteLine($"  PackageId: {package.Manifest.PackageId}");
				output.WriteLine($"  Version: {package.Manifest.Version}");
				output.WriteLine($"  Kind: {package.Manifest.Kind}");
				output.WriteLine($"  SchemaVersion: {package.Manifest.SchemaVersion}");
				if (!string.IsNullOrWhiteSpace(package.Manifest.Description))
				{
					output.WriteLine($"  Description: {package.Manifest.Description}");
				}

				if (package.Manifest.Metadata is not null && package.Manifest.Metadata.Count > 0)
				{
					output.WriteLine("  Metadata:");
					foreach (var kvp in package.Manifest.Metadata.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
					{
						output.WriteLine($"    {kvp.Key}: {kvp.Value}");
					}
				}

				output.WriteLine($"  Artifacts: {package.Entries.Count}");
				foreach (var entry in package.Entries.OrderBy(x => x.Path, StringComparer.OrdinalIgnoreCase))
				{
					output.WriteLine($"    - {entry.Path} ({entry.Length} bytes)");
				}

				output.WriteLine($"  Deployment steps: {package.Manifest.Deployment.Count}");
				foreach (var step in package.Manifest.Deployment)
				{
					var extra = string.Empty;
					if (!string.IsNullOrWhiteSpace(step.Table))
					{
						extra += $" table={step.Table}";
					}
					if (!string.IsNullOrWhiteSpace(step.Mode))
					{
						extra += $" mode={step.Mode}";
					}
					output.WriteLine($"    - {step.Type} -> {step.Artifact}{extra}");
				}

				return Task.FromResult(CommandResult.Success());
			}
			catch (Exception ex)
			{
				return Task.FromResult(CommandResult.Fail($"Unable to inspect package <{command.Path}>: {ex.Message}", ex));
			}
		}
	}
}
