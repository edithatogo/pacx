using System.Text;
using System.Text.Json;

namespace Greg.Xrm.Command.Services.Package
{
	public sealed class PacxPackageReleaser(
		IPacxPackageReader packageReader,
		IPacxPackagePublisher packagePublisher
	) : IPacxPackageReleaser
	{
		private static readonly JsonSerializerOptions SerializerOptions = new()
		{
			WriteIndented = true
		};

		public async Task<PacxPackageReleaseResult> ReleaseAsync(Commands.Package.PackageReleaseCommand command, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (string.IsNullOrWhiteSpace(command.Path))
			{
				throw new ArgumentException("Package path not provided.", nameof(command.Path));
			}

			if (string.IsNullOrWhiteSpace(command.DestinationPath))
			{
				throw new ArgumentException("Destination path not provided.", nameof(command.DestinationPath));
			}

			using var package = packageReader.Open(command.Path);
			var effectiveVersion = string.IsNullOrWhiteSpace(command.Version)
				? package.Manifest.Version
				: command.Version.Trim();
			var releaseDirectoryName = $"{package.Manifest.PackageId}.{effectiveVersion}";
			var destinationRoot = Path.GetFullPath(command.DestinationPath);
			var releaseDirectory = Path.Combine(destinationRoot, releaseDirectoryName);
			if (Directory.Exists(releaseDirectory))
			{
				if (!command.Force)
				{
					throw new InvalidOperationException($"Release directory already exists at <{releaseDirectory}>. Use --force to overwrite it.");
				}

				Directory.Delete(releaseDirectory, recursive: true);
			}

			Directory.CreateDirectory(releaseDirectory);

			var tempPublishRoot = Path.Combine(destinationRoot, ".tmp", $"pacx_release_{Guid.NewGuid():N}");
			Directory.CreateDirectory(tempPublishRoot);

			try
			{
				var published = await packagePublisher.PublishAsync(new Commands.Package.PackagePublishCommand
				{
					Path = command.Path,
					DestinationPath = tempPublishRoot,
					Version = effectiveVersion,
					Force = true
				}, cancellationToken).ConfigureAwait(false);

				var packagePath = MoveFile(published.PackagePath, Path.Combine(releaseDirectory, Path.GetFileName(published.PackagePath)), command.Force);
				var releaseManifestPath = MoveFile(published.ReleaseManifestPath, Path.Combine(releaseDirectory, Path.GetFileName(published.ReleaseManifestPath)), command.Force);
				var provenancePath = Path.Combine(releaseDirectory, "provenance.json");
				var sbomPath = Path.Combine(releaseDirectory, "sbom.json");
				var releaseNotesPath = Path.Combine(releaseDirectory, "RELEASE_NOTES.md");
				WriteProvenance(provenancePath, package.Manifest, effectiveVersion, packagePath, releaseManifestPath, published.ArchiveSha256);
				WriteSbom(sbomPath, package.Manifest, effectiveVersion, packagePath);
				WriteReleaseNotes(releaseNotesPath, package.Manifest, effectiveVersion, packagePath, published.ArchiveSha256);
				var checksumsPath = Path.Combine(releaseDirectory, "checksums.txt");
				WriteChecksums(checksumsPath, packagePath, releaseManifestPath, provenancePath, sbomPath, releaseNotesPath);

				return new PacxPackageReleaseResult(
					releaseDirectory,
					packagePath,
					releaseManifestPath,
					provenancePath,
					sbomPath,
					releaseNotesPath,
					checksumsPath);
			}
			finally
			{
				if (Directory.Exists(tempPublishRoot))
				{
					Directory.Delete(tempPublishRoot, recursive: true);
				}
			}
		}

		private static string MoveFile(string sourcePath, string destinationPath, bool force)
		{
			if (File.Exists(destinationPath))
			{
				if (!force)
				{
					throw new InvalidOperationException($"Release file already exists at <{destinationPath}>. Use --force to overwrite it.");
				}

				File.Delete(destinationPath);
			}

			File.Move(sourcePath, destinationPath);
			return destinationPath;
		}

		private static void WriteReleaseNotes(string releaseNotesPath, PacxPackageManifest manifest, string version, string packagePath, string archiveSha256)
		{
			var sb = new StringBuilder();
			sb.AppendLine($"# {manifest.Name}");
			sb.AppendLine();
			sb.AppendLine($"- PackageId: {manifest.PackageId}");
			sb.AppendLine($"- Version: {version}");
			sb.AppendLine($"- Kind: {manifest.Kind}");
			sb.AppendLine($"- Package: {Path.GetFileName(packagePath)}");
			sb.AppendLine($"- SHA256: {archiveSha256}");
			sb.AppendLine($"- Provenance: provenance.json");
			sb.AppendLine($"- SBOM: sbom.json");
			sb.AppendLine($"- Artifacts: {manifest.Artifacts.Count}");
			sb.AppendLine($"- Deployment steps: {manifest.Deployment.Count}");
			File.WriteAllText(releaseNotesPath, sb.ToString());
		}

		private static void WriteProvenance(
			string provenancePath,
			PacxPackageManifest manifest,
			string version,
			string packagePath,
			string releaseManifestPath,
			string archiveSha256)
		{
			var provenance = new PacxPackageReleaseProvenance
			{
				PackageId = manifest.PackageId,
				Version = version,
				Repository = Environment.GetEnvironmentVariable("GITHUB_REPOSITORY") ?? string.Empty,
				CommitSha = Environment.GetEnvironmentVariable("GITHUB_SHA") ?? string.Empty,
				Workflow = Environment.GetEnvironmentVariable("GITHUB_WORKFLOW") ?? "PACX Package Release",
				RunId = Environment.GetEnvironmentVariable("GITHUB_RUN_ID") ?? string.Empty,
				RunAttempt = Environment.GetEnvironmentVariable("GITHUB_RUN_ATTEMPT") ?? string.Empty,
				ArchivePath = Path.GetFileName(packagePath),
				ArchiveSha256 = archiveSha256,
				ManifestPath = Path.GetFileName(releaseManifestPath),
				ManifestSha256 = ComputeSha256(releaseManifestPath),
				GeneratedAtUtc = DateTimeOffset.UtcNow
			};

			File.WriteAllText(provenancePath, JsonSerializer.Serialize(provenance, SerializerOptions));
		}

		private static void WriteSbom(string sbomPath, PacxPackageManifest manifest, string version, string packagePath)
		{
			var sbom = new PacxPackageReleaseSbom
			{
				PackageId = manifest.PackageId,
				Version = version,
				PackageKind = manifest.Kind,
				PackagePath = Path.GetFileName(packagePath),
				GeneratedAtUtc = DateTimeOffset.UtcNow,
				Components = BuildSbomComponents(manifest, packagePath)
			};

			File.WriteAllText(sbomPath, JsonSerializer.Serialize(sbom, SerializerOptions));
		}

		private static List<PacxPackageSbomComponent> BuildSbomComponents(PacxPackageManifest manifest, string packagePath)
		{
			var components = new List<PacxPackageSbomComponent>
			{
				new() { Type = "archive", Path = Path.GetFileName(packagePath) },
				new() { Type = "manifest", Path = PacxPackageManifest.FileName }
			};

			foreach (var artifact in manifest.Artifacts)
			{
				components.Add(new PacxPackageSbomComponent
				{
					Type = artifact.Role,
					Path = artifact.Path,
					Hash = artifact.Sha256
				});
			}

			return components;
		}

		private static void WriteChecksums(string checksumsPath, params string[] files)
		{
			var sb = new StringBuilder();
			foreach (var file in files)
			{
				var hash = ComputeSha256(file);
				sb.AppendLine($"{hash}  {Path.GetFileName(file)}");
			}

			File.WriteAllText(checksumsPath, sb.ToString());
		}

		private static string ComputeSha256(string path)
		{
			using var stream = File.OpenRead(path);
			var hash = System.Security.Cryptography.SHA256.HashData(stream);
			return Convert.ToHexString(hash).ToLowerInvariant();
		}
	}
}
