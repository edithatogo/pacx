using System.Text.Json;

namespace Greg.Xrm.Command.Services.Package
{
	public sealed class PacxPackageInitializer : IPacxPackageInitializer
	{
		public async Task<string> InitializeAsync(Commands.Package.PackageInitCommand command, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (string.IsNullOrWhiteSpace(command.Path))
			{
				throw new ArgumentException("Package path not provided.", nameof(command.Path));
			}

			var root = Path.GetFullPath(command.Path);
			if (File.Exists(root) && !Directory.Exists(root))
			{
				throw new InvalidOperationException($"Package path <{root}> already exists as a file.");
			}

			if (Directory.Exists(root) && Directory.EnumerateFileSystemEntries(root).Any() && !command.Force)
			{
				throw new InvalidOperationException($"Package folder <{root}> already exists and is not empty. Use --force to overwrite existing files.");
			}

			Directory.CreateDirectory(root);

			var payload = Path.Combine(root, "payload");
			var data = Path.Combine(root, "data");
			var scripts = Path.Combine(root, "scripts");
			Directory.CreateDirectory(payload);
			Directory.CreateDirectory(data);
			Directory.CreateDirectory(scripts);

			var kind = NormalizeKind(command.Kind);
			var manifest = new PacxPackageManifest
			{
				PackageId = NormalizePackageId(command.PackageId ?? Path.GetFileName(root)),
				Version = command.Version,
				Name = command.Name ?? command.PackageId ?? Path.GetFileName(root),
				Description = command.Description,
				Kind = kind,
				Artifacts = [],
				Deployment = [],
				Metadata = new Dictionary<string, string>
				{
					["createdBy"] = "pacx package init"
				}
			};

			switch (kind)
			{
				case PacxPackageKinds.Solution:
					SeedSolutionStarter(payload, manifest);
					break;
				case PacxPackageKinds.Data:
					SeedDataStarter(data, manifest);
					break;
			}

			var manifestPath = Path.Combine(root, PacxPackageManifest.FileName);
			var json = JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true });
			await File.WriteAllTextAsync(manifestPath, json, cancellationToken).ConfigureAwait(false);

			var readmePath = Path.Combine(root, "README.md");
			if (!File.Exists(readmePath) || command.Force)
			{
				var readme = """
				# PACX Package

				This folder was initialized with `pacx package init`.

				- Put deployment payloads in `payload/`
				- Put data files in `data/`
				- Put scripts in `scripts/`
				- Update `pacx.package.json` before building or deploying
				""";
				await File.WriteAllTextAsync(readmePath, readme, cancellationToken).ConfigureAwait(false);
			}

			Touch(Path.Combine(payload, ".gitkeep"), command.Force);
			Touch(Path.Combine(data, ".gitkeep"), command.Force);
			Touch(Path.Combine(scripts, ".gitkeep"), command.Force);

			return root;
		}

		private static void Touch(string path, bool overwrite)
		{
			if (File.Exists(path) && !overwrite)
			{
				return;
			}

			File.WriteAllText(path, string.Empty);
		}

		private static string NormalizePackageId(string value)
		{
			var cleaned = value.Trim().Replace(' ', '-').Replace('_', '-');
			return string.Concat(cleaned.Select(ch => char.IsLetterOrDigit(ch) || ch == '-' || ch == '.' ? char.ToLowerInvariant(ch) : '-'));
		}

		private static string NormalizeKind(string? value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return PacxPackageKinds.Bundle;
			}

			var normalized = value.Trim().ToLowerInvariant();
			if (!PacxPackageKinds.All.Contains(normalized))
			{
				throw new ArgumentOutOfRangeException(nameof(value), $"Unsupported PACX package kind <{value}>. Use one of: {string.Join(", ", PacxPackageKinds.All)}.");
			}

			return normalized;
		}

		private static void SeedSolutionStarter(string payloadRoot, PacxPackageManifest manifest)
		{
			var artifactPath = Path.Combine("payload", "solution.zip");
			CreateEmptyZip(Path.Combine(payloadRoot, "solution.zip"));

			manifest.Artifacts.Add(new PacxPackageArtifact
			{
				Path = artifactPath,
				Role = PacxPackageKinds.Solution,
				Required = true,
				ContentType = "application/zip"
			});

			manifest.Deployment.Add(new PacxPackageDeploymentStep
			{
				Type = "solutionImport",
				Artifact = artifactPath,
				OverwriteUnmanagedCustomizations = true,
				PublishWorkflows = true
			});
		}

		private static void SeedDataStarter(string dataRoot, PacxPackageManifest manifest)
		{
			var artifactPath = Path.Combine("data", "records.json");
			File.WriteAllText(Path.Combine(dataRoot, "records.json"), "[]");

			manifest.Artifacts.Add(new PacxPackageArtifact
			{
				Path = artifactPath,
				Role = PacxPackageKinds.Data,
				Required = true,
				ContentType = "application/json"
			});

			manifest.Deployment.Add(new PacxPackageDeploymentStep
			{
				Type = "dataImport",
				Artifact = artifactPath,
				Mode = "upsert"
			});
		}

		private static void CreateEmptyZip(string path)
		{
			using var stream = File.Create(path);
			using var archive = new System.IO.Compression.ZipArchive(stream, System.IO.Compression.ZipArchiveMode.Create);
		}
	}
}
