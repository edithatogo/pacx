using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;

namespace Greg.Xrm.Command.Services.Package
{
	public sealed class PacxPackagePublisher(
		IPacxPackageReader packageReader,
		IPacxPackageBuilder packageBuilder
	) : IPacxPackagePublisher
	{
		private static readonly JsonSerializerOptions serializerOptions = new()
		{
			WriteIndented = true
		};

		public async Task<PacxPackagePublishResult> PublishAsync(Commands.Package.PackagePublishCommand command, CancellationToken cancellationToken)
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

			var destinationRoot = Path.GetFullPath(command.DestinationPath);
			Directory.CreateDirectory(destinationRoot);

			var archiveName = $"{package.Manifest.PackageId}.{effectiveVersion}.pacx";
			var destinationArchivePath = Path.Combine(destinationRoot, archiveName);
			if (File.Exists(destinationArchivePath))
			{
				if (!command.Force)
				{
					throw new InvalidOperationException($"Published archive already exists at <{destinationArchivePath}>. Use --force to overwrite it.");
				}

				File.Delete(destinationArchivePath);
			}

			string? stagingSourceRoot = null;
			try
			{
				var sourcePath = command.Path;
				if (!string.Equals(effectiveVersion, package.Manifest.Version, StringComparison.OrdinalIgnoreCase))
				{
					stagingSourceRoot = PrepareVersionOverrideSource(command.Path, effectiveVersion, destinationRoot);
					sourcePath = stagingSourceRoot;
				}

				var sourceArchive = await GetArchiveAsync(sourcePath, package.Manifest.PackageId, effectiveVersion, destinationRoot, cancellationToken).ConfigureAwait(false);
				try
				{
					var sourceArchivePath = Path.GetFullPath(sourceArchive.Path);
					var destinationArchiveFullPath = Path.GetFullPath(destinationArchivePath);
					if (!string.Equals(sourceArchivePath, destinationArchiveFullPath, StringComparison.OrdinalIgnoreCase))
					{
						File.Copy(sourceArchivePath, destinationArchiveFullPath, overwrite: true);
					}
				}
				finally
				{
					if (sourceArchive.ShouldDelete)
					{
						File.Delete(sourceArchive.Path);
					}
				}
			}
			finally
			{
				if (stagingSourceRoot != null && Directory.Exists(stagingSourceRoot))
				{
					Directory.Delete(stagingSourceRoot, recursive: true);
				}
			}

			var archiveSha256 = ComputeSha256(destinationArchivePath);
			var releaseManifest = new PacxPackageReleaseManifest
			{
				PackageId = package.Manifest.PackageId,
				Version = effectiveVersion,
				Name = package.Manifest.Name,
				Kind = package.Manifest.Kind,
				SourcePath = Path.GetFullPath(command.Path),
				ArchivePath = destinationArchivePath,
				ArchiveSha256 = archiveSha256,
				ArtifactCount = package.Manifest.Artifacts.Count,
				DeploymentCount = package.Manifest.Deployment.Count,
				PublishedAtUtc = DateTimeOffset.UtcNow
			};

			var releaseManifestPath = Path.Combine(destinationRoot, "pacx.release.json");
			if (File.Exists(releaseManifestPath))
			{
				if (!command.Force)
				{
					throw new InvalidOperationException($"Release manifest already exists at <{releaseManifestPath}>. Use --force to overwrite it.");
				}

				File.Delete(releaseManifestPath);
			}

			File.WriteAllText(releaseManifestPath, JsonSerializer.Serialize(releaseManifest, serializerOptions));
			return new PacxPackagePublishResult(destinationArchivePath, releaseManifestPath, archiveSha256);
		}

		private async Task<PackageArchive> GetArchiveAsync(string sourcePath, string packageId, string version, string destinationRoot, CancellationToken cancellationToken)
		{
			if (File.Exists(sourcePath))
			{
				return new PackageArchive(Path.GetFullPath(sourcePath), ShouldDelete: false);
			}

			var tempArchive = Path.Combine(destinationRoot, $"{packageId}.{version}.{Guid.NewGuid():N}.pacx");
			var built = await packageBuilder.BuildAsync(sourcePath, tempArchive, cancellationToken).ConfigureAwait(false);
			return new PackageArchive(built, ShouldDelete: true);
		}

		private static string PrepareVersionOverrideSource(string sourcePath, string version, string destinationRoot)
		{
			var tempSourceRoot = Path.Combine(destinationRoot, ".tmp", $"pacx_publish_source_{Guid.NewGuid():N}");
			Directory.CreateDirectory(tempSourceRoot);

			if (Directory.Exists(sourcePath))
			{
				CopyDirectory(sourcePath, tempSourceRoot);
			}
			else
			{
				ZipFile.ExtractToDirectory(sourcePath, tempSourceRoot, overwriteFiles: true);
			}

			var manifestPath = Path.Combine(tempSourceRoot, PacxPackageManifest.FileName);
			var manifest = JsonSerializer.Deserialize<PacxPackageManifest>(File.ReadAllText(manifestPath), serializerOptions)
				?? throw new InvalidDataException($"Manifest file <{manifestPath}> could not be parsed.");
			manifest.Version = version;
			File.WriteAllText(manifestPath, JsonSerializer.Serialize(manifest, serializerOptions));

			return tempSourceRoot;
		}

		private static void CopyDirectory(string sourcePath, string destinationPath)
		{
			Directory.CreateDirectory(destinationPath);

			foreach (var directory in Directory.EnumerateDirectories(sourcePath, "*", SearchOption.AllDirectories))
			{
				var relative = Path.GetRelativePath(sourcePath, directory);
				Directory.CreateDirectory(Path.Combine(destinationPath, relative));
			}

			foreach (var file in Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories))
			{
				var relative = Path.GetRelativePath(sourcePath, file);
				var target = Path.Combine(destinationPath, relative);
				var targetDirectory = Path.GetDirectoryName(target);
				if (!string.IsNullOrWhiteSpace(targetDirectory))
				{
					Directory.CreateDirectory(targetDirectory);
				}

				File.Copy(file, target, overwrite: true);
			}
		}

		private static string ComputeSha256(string path)
		{
			using var stream = File.OpenRead(path);
			var hash = SHA256.HashData(stream);
			return Convert.ToHexString(hash).ToLowerInvariant();
		}

		private sealed record PackageArchive(string Path, bool ShouldDelete);
	}
}
