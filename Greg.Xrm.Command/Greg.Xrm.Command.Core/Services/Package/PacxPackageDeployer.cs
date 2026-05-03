using System.Globalization;
using System.Text.Json;
using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Output;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

namespace Greg.Xrm.Command.Services.Package
{
	public sealed class PacxPackageDeployer(
		IOutput output,
		IOrganizationServiceRepository organizationServiceRepository
	) : IPacxPackageDeployer
	{
		public async Task DeployAsync(IPacxPackageSource source, bool dryRun, CancellationToken cancellationToken)
		{
			output.WriteLine($"PACX package: {source.Manifest.Name} ({source.Manifest.PackageId} {source.Manifest.Version})", ConsoleColor.Cyan);
			output.WriteLine($"  Kind: {source.Manifest.Kind}");
			output.WriteLine($"  Source: {source.SourcePath}");
			output.WriteLine($"  Artifacts: {source.Entries.Count}");
			output.WriteLine($"  Deployment steps: {source.Manifest.Deployment.Count}");
			PrintPlan(source);

			if (dryRun)
			{
				output.WriteLine("[DRY RUN] No changes will be applied.", ConsoleColor.Yellow);
				return;
			}

			output.Write("Connecting to Dataverse...");
			var crm = await organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			output.WriteLine(" Done", ConsoleColor.Green);

			foreach (var step in source.Manifest.Deployment)
			{
				switch (Normalize(step.Type))
				{
					case "solutionimport":
						await ImportSolutionAsync(crm, source, step, cancellationToken).ConfigureAwait(false);
						break;
					case "dataimport":
						await ImportDataAsync(crm, source, step, cancellationToken).ConfigureAwait(false);
						break;
					default:
						throw new InvalidDataException($"Unsupported deployment step type <{step.Type}>.");
				}
			}
		}

		private void PrintPlan(IPacxPackageSource source)
		{
			if (source.Manifest.Deployment.Count == 0)
			{
				output.WriteLine("  No deployment steps declared.");
				return;
			}

			output.WriteLine("  Deployment plan:");
			output.WriteTable(source.Manifest.Deployment,
				() => ["Type", "Artifact", "Status", "Table", "Mode", "Flags"],
				step =>
				{
					var status = GetStatus(source, step);
					return [
						step.Type,
						step.Artifact,
						status,
						step.Table ?? "",
						step.Mode ?? "",
						BuildFlags(step)
					];
				},
				(_, step) => GetStatus(source, step) switch
				{
					"ready" => ConsoleColor.Green,
					"missing" => ConsoleColor.Red,
					_ => ConsoleColor.Yellow
				});
		}

		private async Task ImportSolutionAsync(IOrganizationServiceAsync2 crm, IPacxPackageSource source, PacxPackageDeploymentStep step, CancellationToken cancellationToken)
		{
			var artifactPath = RequireArtifact(source, step);
			output.Write($"Importing solution artifact {artifactPath}...");

			using var stream = source.OpenRead(artifactPath);
			using var memoryStream = new MemoryStream();
			await stream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);

			var request = new ImportSolutionRequest
			{
				CustomizationFile = memoryStream.ToArray(),
				OverwriteUnmanagedCustomizations = step.OverwriteUnmanagedCustomizations ?? true,
				PublishWorkflows = step.PublishWorkflows ?? true
			};

			await crm.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
			output.WriteLine(" Done", ConsoleColor.Green);
		}

		private async Task ImportDataAsync(IOrganizationServiceAsync2 crm, IPacxPackageSource source, PacxPackageDeploymentStep step, CancellationToken cancellationToken)
		{
			var artifactPath = RequireArtifact(source, step);
			output.Write($"Importing data artifact {artifactPath}...");

			using var stream = source.OpenRead(artifactPath);
			var records = await JsonSerializer.DeserializeAsync<List<Dictionary<string, JsonElement>>>(stream, new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			}, cancellationToken).ConfigureAwait(false);

			if (records == null)
			{
				throw new InvalidDataException($"Data artifact <{artifactPath}> could not be parsed.");
			}

			var tableName = step.Table;
			if (string.IsNullOrWhiteSpace(tableName))
			{
				tableName = Path.GetFileNameWithoutExtension(artifactPath);
			}

			var mode = Normalize(step.Mode ?? "upsert");
			var primaryKey = $"{tableName}id";
			var count = 0;

			foreach (var record in records)
			{
				var entity = new Entity(tableName);
				foreach (var kvp in record)
				{
					entity[kvp.Key] = DeserializeValue(kvp.Value);
				}

				if (mode == "upsert" && TryGetGuid(entity, primaryKey, out var id))
				{
					entity.Id = id;
					await crm.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
				}
				else
				{
					await crm.CreateAsync(entity, cancellationToken).ConfigureAwait(false);
				}

				count++;
			}

			output.WriteLine($" Done ({count} record(s))", ConsoleColor.Green);
		}

		private static string RequireArtifact(IPacxPackageSource source, PacxPackageDeploymentStep step)
		{
			var path = PacxPackagePath.NormalizePath(step.Artifact);
			if (!source.Exists(path))
			{
				throw new FileNotFoundException($"Artifact <{path}> does not exist in package <{source.SourcePath}>.");
			}

			return path;
		}

		private static string GetStatus(IPacxPackageSource source, PacxPackageDeploymentStep step)
		{
			var type = Normalize(step.Type);
			if (type != "solutionimport" && type != "dataimport")
			{
				return "unsupported";
			}

			return source.Exists(step.Artifact) ? "ready" : "missing";
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

		private static object? DeserializeValue(JsonElement element)
		{
			return element.ValueKind switch
			{
				JsonValueKind.String => TryParseGuid(element.GetString(), out var guid) ? guid : element.GetString(),
				JsonValueKind.Number => element.TryGetInt64(out var longValue) ? longValue : element.GetDouble(),
				JsonValueKind.True => true,
				JsonValueKind.False => false,
				JsonValueKind.Null => null,
				JsonValueKind.Object => DeserializeObject(element),
				JsonValueKind.Array => element.EnumerateArray().Select(DeserializeValue).ToArray(),
				_ => element.GetRawText()
			};
		}

		private static object? DeserializeObject(JsonElement element)
		{
			if (element.TryGetProperty("Id", out var idProp) && element.TryGetProperty("LogicalName", out var logicalNameProp) && idProp.ValueKind == JsonValueKind.String)
			{
				if (Guid.TryParse(idProp.GetString(), out var guid))
				{
					return new EntityReference(logicalNameProp.GetString(), guid);
				}
			}

			return element.GetRawText();
		}

		private static bool TryGetGuid(Entity entity, string attributeName, out Guid id)
		{
			id = Guid.Empty;
			if (!entity.Attributes.TryGetValue(attributeName, out var value) || value == null)
			{
				return false;
			}

			if (value is Guid guid)
			{
				id = guid;
				return true;
			}

			if (value is string text && Guid.TryParse(text, out guid))
			{
				id = guid;
				return true;
			}

			return false;
		}

		private static bool TryParseGuid(string? text, out Guid guid)
		{
			return Guid.TryParse(text, out guid);
		}

		private static string Normalize(string text)
		{
			return text.Trim().Replace("-", string.Empty).ToLowerInvariant();
		}
	}
}
