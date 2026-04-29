using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Greg.Xrm.Command.Services.Package
{
	public sealed class PacxPackageAuthoringService : IPacxPackageAuthoringService
	{
		private static readonly JsonSerializerOptions serializerOptions = new()
		{
			WriteIndented = true
		};

		public async Task<string> AddSolutionAsync(Commands.Package.PackageAddSolutionCommand command, CancellationToken cancellationToken)
		{
			var root = EnsurePackageFolder(command.Path);
			using var package = LoadPackage(root);

			var source = EnsureSourceFile(command.SourcePath, ".zip");
			var artifactPath = NormalizeArtifactPath(command.ArtifactPath, "payload", Path.GetFileName(source));
			var destination = GetDestinationPath(root, artifactPath);

			CopyFile(source, destination, command.Force);
			UpsertArtifact(package.Manifest, artifactPath, "solution", "application/zip", required: true);
			UpsertDeploymentStep(package.Manifest, new PacxPackageDeploymentStep
			{
				Type = "solutionImport",
				Artifact = artifactPath,
				OverwriteUnmanagedCustomizations = command.OverwriteUnmanagedCustomizations,
				PublishWorkflows = command.PublishWorkflows
			});

			NormalizeKind(package.Manifest);
			SaveManifest(root, package.Manifest);
			return artifactPath;
		}

		public async Task<string> AddDataAsync(Commands.Package.PackageAddDataCommand command, CancellationToken cancellationToken)
		{
			var root = EnsurePackageFolder(command.Path);
			using var package = LoadPackage(root);

			var source = EnsureSourceFile(command.SourcePath, ".json");
			var artifactPath = NormalizeArtifactPath(command.ArtifactPath, "data", Path.GetFileName(source));
			var destination = GetDestinationPath(root, artifactPath);

			CopyFile(source, destination, command.Force);
			var tableName = NormalizeTableName(command.Table ?? Path.GetFileNameWithoutExtension(source));
			UpsertArtifact(package.Manifest, artifactPath, "data", "application/json", required: true);
			UpsertDeploymentStep(package.Manifest, new PacxPackageDeploymentStep
			{
				Type = "dataImport",
				Artifact = artifactPath,
				Table = tableName,
				Mode = string.IsNullOrWhiteSpace(command.Mode) ? "upsert" : command.Mode
			});

			NormalizeKind(package.Manifest);
			SaveManifest(root, package.Manifest);
			return artifactPath;
		}

		public async Task<string> RemoveSolutionAsync(Commands.Package.PackageRemoveSolutionCommand command, CancellationToken cancellationToken)
		{
			return await RemoveArtifactAsync(
				command.Path,
				command.ArtifactPath,
				command.Force,
				"solutionImport",
				cancellationToken).ConfigureAwait(false);
		}

		public async Task<string> RemoveDataAsync(Commands.Package.PackageRemoveDataCommand command, CancellationToken cancellationToken)
		{
			return await RemoveArtifactAsync(
				command.Path,
				command.ArtifactPath,
				command.Force,
				"dataImport",
				cancellationToken).ConfigureAwait(false);
		}

		public async Task<PacxPackageSyncResult> SyncAsync(Commands.Package.PackageSyncCommand command, CancellationToken cancellationToken)
		{
			var root = EnsurePackageFolder(command.Path);
			var manifest = LoadManifest(root);

			var addedArtifacts = 0;
			var addedSteps = 0;
			var prunedArtifacts = 0;
			var prunedSteps = 0;

			var discovered = DiscoverPackageFiles(root).ToList();
			var discoveredPaths = discovered.Select(x => x.ArtifactPath).ToHashSet(StringComparer.OrdinalIgnoreCase);

			foreach (var file in discovered)
			{
				if (!manifest.Artifacts.Any(x => string.Equals(x.Path, file.ArtifactPath, StringComparison.OrdinalIgnoreCase)))
				{
					manifest.Artifacts.Add(new PacxPackageArtifact
					{
						Path = file.ArtifactPath,
						Role = file.Role,
						Required = file.Required,
						ContentType = file.ContentType
					});
					addedArtifacts++;
				}

				if (file.Step is not null && !manifest.Deployment.Any(x => string.Equals(x.Type, file.Step.Type, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Artifact, file.Step.Artifact, StringComparison.OrdinalIgnoreCase)))
				{
					manifest.Deployment.Add(file.Step);
					addedSteps++;
				}
			}

			if (command.PruneMissing)
			{
				prunedArtifacts += manifest.Artifacts.RemoveAll(x => IsManagedArtifact(x.Path) && !discoveredPaths.Contains(x.Path));
				prunedSteps += manifest.Deployment.RemoveAll(x => IsManagedArtifact(x.Artifact) && !discoveredPaths.Contains(x.Artifact));
			}

			NormalizeKind(manifest);
			SaveManifest(root, manifest);
			await Task.CompletedTask.ConfigureAwait(false);
			return new PacxPackageSyncResult(addedArtifacts, addedSteps, prunedArtifacts, prunedSteps);
		}

		public async Task<PacxPackageFixResult> FixAsync(Commands.Package.PackageFixCommand command, CancellationToken cancellationToken)
		{
			var root = EnsurePackageFolder(command.Path);
			var manifest = LoadManifest(root);

			var addedArtifacts = 0;
			var addedSteps = 0;
			var dedupedArtifacts = 0;
			var dedupedSteps = 0;
			var prunedArtifacts = 0;
			var prunedSteps = 0;

			var discovered = DiscoverPackageFiles(root).ToList();
			var discoveredPaths = discovered.Select(x => x.ArtifactPath).ToHashSet(StringComparer.OrdinalIgnoreCase);

			foreach (var file in discovered)
			{
				if (!manifest.Artifacts.Any(x => string.Equals(x.Path, file.ArtifactPath, StringComparison.OrdinalIgnoreCase)))
				{
					manifest.Artifacts.Add(new PacxPackageArtifact
					{
						Path = file.ArtifactPath,
						Role = file.Role,
						Required = file.Required,
						ContentType = file.ContentType
					});
					addedArtifacts++;
				}

				if (file.Step is not null && !manifest.Deployment.Any(x => string.Equals(x.Type, file.Step.Type, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Artifact, file.Step.Artifact, StringComparison.OrdinalIgnoreCase)))
				{
					manifest.Deployment.Add(file.Step);
					addedSteps++;
				}
			}

			dedupedArtifacts += NormalizeArtifacts(manifest.Artifacts);
			dedupedSteps += NormalizeDeployment(manifest.Deployment);

			if (command.PruneMissing)
			{
				prunedArtifacts += manifest.Artifacts.RemoveAll(x => IsManagedArtifact(x.Path) && !discoveredPaths.Contains(x.Path));
				prunedSteps += manifest.Deployment.RemoveAll(x => IsManagedArtifact(x.Artifact) && !discoveredPaths.Contains(x.Artifact));
			}

			manifest.Artifacts = manifest.Artifacts
				.OrderBy(x => x.Path, StringComparer.OrdinalIgnoreCase)
				.ToList();
			manifest.Deployment = manifest.Deployment
				.OrderBy(x => x.Type, StringComparer.OrdinalIgnoreCase)
				.ThenBy(x => x.Artifact, StringComparer.OrdinalIgnoreCase)
				.ToList();

			NormalizeKind(manifest);
			SaveManifest(root, manifest);
			await Task.CompletedTask.ConfigureAwait(false);
			return new PacxPackageFixResult(addedArtifacts, addedSteps, dedupedArtifacts, dedupedSteps, prunedArtifacts, prunedSteps);
		}

		private static string EnsurePackageFolder(string path)
		{
			var root = Path.GetFullPath(path);
			if (!Directory.Exists(root))
			{
				throw new DirectoryNotFoundException($"Package folder <{root}> does not exist.");
			}

			return root;
		}

		private static IPacxPackageSource LoadPackage(string root)
		{
			var reader = new PacxPackageReader();
			return reader.Open(root);
		}

		private static string EnsureSourceFile(string sourcePath, string requiredExtension)
		{
			if (string.IsNullOrWhiteSpace(sourcePath))
			{
				throw new ArgumentException("Source path not provided.", nameof(sourcePath));
			}

			var fullPath = Path.GetFullPath(sourcePath);
			if (!File.Exists(fullPath))
			{
				throw new FileNotFoundException($"Source file <{fullPath}> does not exist.", fullPath);
			}

			if (!string.Equals(Path.GetExtension(fullPath), requiredExtension, StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException($"Source file <{fullPath}> must have extension <{requiredExtension}>.");
			}

			return fullPath;
		}

		private static string NormalizeArtifactPath(string? artifactPath, string folder, string fileName)
		{
			var relative = string.IsNullOrWhiteSpace(artifactPath)
				? Path.Combine(folder, fileName)
				: artifactPath;

			return PacxPackagePath.NormalizePath(relative);
		}

		private static string GetDestinationPath(string root, string artifactPath)
		{
			var normalizedRoot = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
			var fullPath = Path.GetFullPath(Path.Combine(root, artifactPath.Replace('/', Path.DirectorySeparatorChar)));
			if (!fullPath.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException($"Artifact path <{artifactPath}> must stay inside the package folder.");
			}

			var directory = Path.GetDirectoryName(fullPath);
			if (!string.IsNullOrWhiteSpace(directory))
			{
				Directory.CreateDirectory(directory);
			}

			return fullPath;
		}

		private static void CopyFile(string source, string destination, bool force)
		{
			if (File.Exists(destination) && !force)
			{
				throw new InvalidOperationException($"Artifact already exists at <{destination}>. Use --force to overwrite it.");
			}

			File.Copy(source, destination, overwrite: true);
		}

		private static void UpsertArtifact(PacxPackageManifest manifest, string path, string role, string contentType, bool required)
		{
			var existing = manifest.Artifacts.FirstOrDefault(x => string.Equals(x.Path, path, StringComparison.OrdinalIgnoreCase));
			if (existing == null)
			{
				manifest.Artifacts.Add(new PacxPackageArtifact
				{
					Path = path,
					Role = role,
					Required = required,
					ContentType = contentType
				});
				return;
			}

			existing.Role = role;
			existing.Required = required;
			existing.ContentType = contentType;
		}

		private static void UpsertDeploymentStep(PacxPackageManifest manifest, PacxPackageDeploymentStep step)
		{
			var existing = manifest.Deployment.FirstOrDefault(x => string.Equals(x.Artifact, step.Artifact, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Type, step.Type, StringComparison.OrdinalIgnoreCase));
			if (existing == null)
			{
				manifest.Deployment.Add(step);
				return;
			}

			existing.Table = step.Table;
			existing.Mode = step.Mode;
			existing.OverwriteUnmanagedCustomizations = step.OverwriteUnmanagedCustomizations;
			existing.PublishWorkflows = step.PublishWorkflows;
		}

		private static int NormalizeArtifacts(List<PacxPackageArtifact> artifacts)
		{
			var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			var removed = 0;

			for (var i = artifacts.Count - 1; i >= 0; i--)
			{
				var artifact = artifacts[i];
				artifact.Path = PacxPackagePath.NormalizePath(artifact.Path);

				if (!seen.Add(artifact.Path))
				{
					artifacts.RemoveAt(i);
					removed++;
				}
			}

			return removed;
		}

		private static int NormalizeDeployment(List<PacxPackageDeploymentStep> steps)
		{
			var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			var removed = 0;

			for (var i = steps.Count - 1; i >= 0; i--)
			{
				var step = steps[i];
				step.Artifact = PacxPackagePath.NormalizePath(step.Artifact);
				var key = $"{step.Type}|{step.Artifact}";
				if (!seen.Add(key))
				{
					steps.RemoveAt(i);
					removed++;
				}
			}

			return removed;
		}

		private static async Task<string> RemoveArtifactAsync(string packagePath, string artifactPath, bool force, string stepType, CancellationToken cancellationToken)
		{
			var root = EnsurePackageFolder(packagePath);
			var manifest = LoadManifest(root);

			var normalizedArtifactPath = PacxPackagePath.NormalizePath(artifactPath);
			var destination = Path.GetFullPath(Path.Combine(root, normalizedArtifactPath.Replace('/', Path.DirectorySeparatorChar)));
			var normalizedRoot = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
			if (!destination.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException($"Artifact path <{artifactPath}> must stay inside the package folder.");
			}

			if (File.Exists(destination))
			{
				File.Delete(destination);
			}
			else if (!force)
			{
				throw new FileNotFoundException($"Artifact file <{destination}> does not exist. Use --force to remove manifest entries anyway.");
			}

			manifest.Artifacts.RemoveAll(x => string.Equals(x.Path, normalizedArtifactPath, StringComparison.OrdinalIgnoreCase));
			manifest.Deployment.RemoveAll(x => string.Equals(x.Artifact, normalizedArtifactPath, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Type, stepType, StringComparison.OrdinalIgnoreCase));

			NormalizeKind(manifest);
			SaveManifest(root, manifest);
			await Task.CompletedTask.ConfigureAwait(false);
			return normalizedArtifactPath;
		}

		private static PacxPackageManifest LoadManifest(string root)
		{
			var manifestPath = Path.Combine(root, PacxPackageManifest.FileName);
			if (!File.Exists(manifestPath))
			{
				throw new FileNotFoundException($"Manifest file <{manifestPath}> was not found.");
			}

			var json = File.ReadAllText(manifestPath);
			var manifest = JsonSerializer.Deserialize<PacxPackageManifest>(json, serializerOptions);
			if (manifest == null)
			{
				throw new InvalidDataException($"Manifest file <{manifestPath}> could not be parsed.");
			}

			return manifest;
		}

		private static void SaveManifest(string root, PacxPackageManifest manifest)
		{
			var manifestPath = Path.Combine(root, PacxPackageManifest.FileName);
			var json = JsonSerializer.Serialize(manifest, serializerOptions);
			File.WriteAllText(manifestPath, json);
		}

		private static string NormalizeTableName(string value)
		{
			return value.Trim();
		}

		private static void NormalizeKind(PacxPackageManifest manifest)
		{
			var hasSolutionArtifact = manifest.Artifacts.Any(x =>
				string.Equals(x.Role, PacxPackageKinds.Solution, StringComparison.OrdinalIgnoreCase) ||
				string.Equals(x.Role, "solution", StringComparison.OrdinalIgnoreCase));
			var hasDataArtifact = manifest.Artifacts.Any(x => string.Equals(x.Role, PacxPackageKinds.Data, StringComparison.OrdinalIgnoreCase));
			var hasSolutionStep = manifest.Deployment.Any(x => string.Equals(x.Type, "solutionImport", StringComparison.OrdinalIgnoreCase));
			var hasDataStep = manifest.Deployment.Any(x => string.Equals(x.Type, "dataImport", StringComparison.OrdinalIgnoreCase));

			if ((hasSolutionArtifact || hasSolutionStep) && (hasDataArtifact || hasDataStep))
			{
				manifest.Kind = PacxPackageKinds.Bundle;
				return;
			}

			if (hasSolutionArtifact || hasSolutionStep)
			{
				manifest.Kind = PacxPackageKinds.Solution;
				return;
			}

			if (hasDataArtifact || hasDataStep)
			{
				manifest.Kind = PacxPackageKinds.Data;
				return;
			}

			manifest.Kind = PacxPackageKinds.Bundle;
		}

		private static IEnumerable<DiscoveredFile> DiscoverPackageFiles(string root)
		{
			foreach (var file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
			{
				var relativePath = Path.GetRelativePath(root, file).Replace('\\', '/');
				if (string.Equals(relativePath, PacxPackageManifest.FileName, StringComparison.OrdinalIgnoreCase) ||
					string.Equals(Path.GetFileName(relativePath), ".gitkeep", StringComparison.OrdinalIgnoreCase) ||
					string.Equals(Path.GetFileName(relativePath), "README.md", StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}

				var normalized = PacxPackagePath.NormalizePath(relativePath);
				var topLevel = normalized.Split('/', 2, StringSplitOptions.RemoveEmptyEntries)[0];
				if (!string.Equals(topLevel, "payload", StringComparison.OrdinalIgnoreCase) &&
					!string.Equals(topLevel, "data", StringComparison.OrdinalIgnoreCase) &&
					!string.Equals(topLevel, "scripts", StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}

				yield return new DiscoveredFile
				{
					ArtifactPath = normalized,
					Role = InferRole(normalized),
					Required = !string.Equals(topLevel, "scripts", StringComparison.OrdinalIgnoreCase),
					ContentType = InferContentType(normalized),
					Step = InferDeploymentStep(normalized)
				};
			}
		}

		private static bool IsManagedArtifact(string path)
		{
			return path.StartsWith("payload/", StringComparison.OrdinalIgnoreCase) ||
				path.StartsWith("data/", StringComparison.OrdinalIgnoreCase) ||
				path.StartsWith("scripts/", StringComparison.OrdinalIgnoreCase);
		}

		private static string InferRole(string path)
		{
			if (path.StartsWith("payload/", StringComparison.OrdinalIgnoreCase))
			{
				return "solution";
			}

			if (path.StartsWith("data/", StringComparison.OrdinalIgnoreCase))
			{
				return "data";
			}

			return "script";
		}

		private static string? InferContentType(string path)
		{
			if (path.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
			{
				return "application/zip";
			}

			if (path.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
			{
				return "application/json";
			}

			if (path.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
			{
				return "text/javascript";
			}

			return null;
		}

		private static PacxPackageDeploymentStep? InferDeploymentStep(string path)
		{
			if (path.StartsWith("payload/", StringComparison.OrdinalIgnoreCase) && path.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
			{
				return new PacxPackageDeploymentStep
				{
					Type = "solutionImport",
					Artifact = path,
					OverwriteUnmanagedCustomizations = true,
					PublishWorkflows = true
				};
			}

			if (path.StartsWith("data/", StringComparison.OrdinalIgnoreCase) && path.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
			{
				return new PacxPackageDeploymentStep
				{
					Type = "dataImport",
					Artifact = path,
					Table = Path.GetFileNameWithoutExtension(path),
					Mode = "upsert"
				};
			}

			return null;
		}

		private sealed class DiscoveredFile
		{
			public string ArtifactPath { get; set; } = string.Empty;

			public string Role { get; set; } = string.Empty;

			public bool Required { get; set; }

			public string? ContentType { get; set; }

			public PacxPackageDeploymentStep? Step { get; set; }
		}
	}
}
