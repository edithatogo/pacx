using System.Collections.Generic;
using System.Linq;
using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Package;

namespace Greg.Xrm.Command.Commands.Package
{
	public sealed class PackageListCommandExecutor(
		IOutput output,
		IPacxPackageReader packageReader
	) : ICommandExecutor<PackageListCommand>
	{
		public Task<CommandResult> ExecuteAsync(PackageListCommand command, CancellationToken cancellationToken)
		{
			try
			{
				using var package = packageReader.Open(command.Path);
				var entries = package.Entries.ToDictionary(x => x.Path, StringComparer.OrdinalIgnoreCase);

				output.WriteLine($"PACX package list: {package.Manifest.Name}", ConsoleColor.Cyan);
				output.WriteLine($"  PackageId: {package.Manifest.PackageId}");
				output.WriteLine($"  Version: {package.Manifest.Version}");
				output.WriteLine($"  Kind: {PacxPackageKinds.Describe(package.Manifest.Kind)}");
				output.WriteLine($"  Contract: {PacxPackageKinds.DescribeContract(package.Manifest.Kind)}");
				output.WriteLine($"  SchemaVersion: {package.Manifest.SchemaVersion}");

				if (!string.IsNullOrWhiteSpace(package.Manifest.Description))
				{
					output.WriteLine($"  Description: {package.Manifest.Description}");
				}

				if (package.Manifest.Metadata is { Count: > 0 })
				{
					output.WriteLine("  Metadata:");
					foreach (var kvp in package.Manifest.Metadata.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
					{
						output.WriteLine($"    {kvp.Key}: {kvp.Value}");
					}
				}

				output.WriteLine("  Artifacts:");
				output.WriteTable(package.Manifest.Artifacts,
					() => ["Path", "Role", "Required", "Present", "Bytes", "ContentType", "Sha256"],
					artifact =>
					{
						var present = entries.TryGetValue(artifact.Path, out var entry);
						return [
							artifact.Path,
							artifact.Role,
							artifact.Required ? "yes" : "no",
							present ? "yes" : "no",
							present && entry is not null ? entry.Length.ToString() : "",
							artifact.ContentType ?? "",
							artifact.Sha256 ?? ""
						];
					},
					(_, artifact) =>
					{
						if (artifact.Required && !entries.ContainsKey(artifact.Path))
						{
							return ConsoleColor.Red;
						}

						if (!entries.ContainsKey(artifact.Path))
						{
							return ConsoleColor.Yellow;
						}

						return ConsoleColor.Green;
					});

				output.WriteLine($"  Deployment steps: {package.Manifest.Deployment.Count}");
				output.WriteTable(package.Manifest.Deployment,
					() => ["Type", "Artifact", "Status", "Table", "Mode", "Flags"],
					step =>
					{
						var present = entries.ContainsKey(step.Artifact);
						return [
							step.Type,
							step.Artifact,
							present ? "ready" : "missing",
							step.Table ?? "",
							step.Mode ?? "",
							BuildFlags(step)
						];
					},
					(_, step) => entries.ContainsKey(step.Artifact) ? ConsoleColor.Green : ConsoleColor.Red);

				return Task.FromResult(CommandResult.Success());
			}
			catch (Exception ex)
			{
				return Task.FromResult(CommandResult.Fail($"Unable to list package <{command.Path}>: {ex.Message}", ex));
			}
		}

		private static string BuildFlags(PacxPackageDeploymentStep step)
		{
			var flags = new List<string>();

			if (step.OverwriteUnmanagedCustomizations.HasValue)
			{
				flags.Add($"overwriteUnmanagedCustomizations={step.OverwriteUnmanagedCustomizations.Value}");
			}

			if (step.PublishWorkflows.HasValue)
			{
				flags.Add($"publishWorkflows={step.PublishWorkflows.Value}");
			}

			if (step.Extensions is { Count: > 0 })
			{
				flags.Add($"{step.Extensions.Count} extension(s)");
			}

			return string.Join(", ", flags);
		}
	}
}
