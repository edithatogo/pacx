using System.Security.Cryptography;
using System.Text.Json;

namespace Greg.Xrm.Command.Services.Package
{
	public interface IPacxPackageReleaseVerifier
	{
		PacxPackageReleaseVerificationResult Verify(string releaseDirectory);
	}

	public sealed class PacxPackageReleaseVerificationResult
	{
		public PacxPackageReleaseVerificationResult(
			IReadOnlyList<string> missingFiles,
			IReadOnlyList<string> checksumErrors,
			Exception? exception = null)
		{
			MissingFiles = missingFiles ?? throw new ArgumentNullException(nameof(missingFiles));
			ChecksumErrors = checksumErrors ?? throw new ArgumentNullException(nameof(checksumErrors));
			Exception = exception;
		}

		public IReadOnlyList<string> MissingFiles { get; }

		public IReadOnlyList<string> ChecksumErrors { get; }

		public Exception? Exception { get; }

		public bool IsValid => Exception is null && MissingFiles.Count == 0 && ChecksumErrors.Count == 0;
	}

	public sealed class PacxPackageReleaseVerifier : IPacxPackageReleaseVerifier
	{
		private const string ReleaseManifestFileName = "pacx.release.json";
		private const string ProvenanceFileName = "provenance.json";
		private const string SbomFileName = "sbom.json";
		private const string ReleaseNotesFileName = "RELEASE_NOTES.md";
		private const string ChecksumsFileName = "checksums.txt";

		public PacxPackageReleaseVerificationResult Verify(string releaseDirectory)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(releaseDirectory))
				{
					throw new ArgumentException("Release directory is required.", nameof(releaseDirectory));
				}

				if (!Directory.Exists(releaseDirectory))
				{
					throw new DirectoryNotFoundException($"Release directory does not exist: {releaseDirectory}");
				}

				var missingFiles = new List<string>();
				var checksumErrors = new List<string>();

				var packageFiles = Directory.GetFiles(releaseDirectory, "*.pacx", SearchOption.TopDirectoryOnly);
				var releaseManifestPath = Path.Combine(releaseDirectory, ReleaseManifestFileName);
				var provenancePath = Path.Combine(releaseDirectory, ProvenanceFileName);
				var sbomPath = Path.Combine(releaseDirectory, SbomFileName);
				var releaseNotesPath = Path.Combine(releaseDirectory, ReleaseNotesFileName);
				var checksumsPath = Path.Combine(releaseDirectory, ChecksumsFileName);

				if (packageFiles.Length == 0)
				{
					missingFiles.Add("*.pacx");
				}

				if (!File.Exists(releaseManifestPath))
				{
					missingFiles.Add(ReleaseManifestFileName);
				}

				if (!File.Exists(provenancePath))
				{
					missingFiles.Add(ProvenanceFileName);
				}

				if (!File.Exists(sbomPath))
				{
					missingFiles.Add(SbomFileName);
				}

				if (!File.Exists(releaseNotesPath))
				{
					missingFiles.Add(ReleaseNotesFileName);
				}

				if (!File.Exists(checksumsPath))
				{
					missingFiles.Add(ChecksumsFileName);
				}

				if (missingFiles.Count > 0)
				{
					return new PacxPackageReleaseVerificationResult(missingFiles, checksumErrors);
				}

				var manifest = JsonSerializer.Deserialize<PacxPackageReleaseManifest>(File.ReadAllText(releaseManifestPath));
				if (manifest is null)
				{
					throw new InvalidDataException($"Unable to parse release manifest at <{releaseManifestPath}>.");
				}

				var provenance = JsonSerializer.Deserialize<PacxPackageReleaseProvenance>(File.ReadAllText(provenancePath));
				if (provenance is null)
				{
					throw new InvalidDataException($"Unable to parse provenance file at <{provenancePath}>.");
				}

				var sbom = JsonSerializer.Deserialize<PacxPackageReleaseSbom>(File.ReadAllText(sbomPath));
				if (sbom is null)
				{
					throw new InvalidDataException($"Unable to parse SBOM file at <{sbomPath}>.");
				}

				var packagePath = packageFiles.Single();
				if (!string.Equals(Path.GetFileName(packagePath), Path.GetFileName(manifest.ArchivePath), StringComparison.OrdinalIgnoreCase))
				{
					checksumErrors.Add($"ArchivePath does not match package file name: {manifest.ArchivePath}");
				}

				if (!string.Equals(provenance.PackageId, manifest.PackageId, StringComparison.OrdinalIgnoreCase))
				{
					checksumErrors.Add("Provenance package id does not match the release manifest.");
				}

				if (!string.Equals(provenance.ArchiveSha256, manifest.ArchiveSha256, StringComparison.OrdinalIgnoreCase))
				{
					checksumErrors.Add("Provenance archive hash does not match the release manifest.");
				}

				if (!string.Equals(sbom.PackageId, manifest.PackageId, StringComparison.OrdinalIgnoreCase))
				{
					checksumErrors.Add("SBOM package id does not match the release manifest.");
				}

				VerifyProvenance(provenance, manifest, releaseManifestPath, packagePath, checksumErrors);

				// Create a package manifest from the release manifest for SBOM verification
				var packageManifest = new PacxPackageManifest
				{
					PackageId = manifest.PackageId,
					Version = manifest.Version,
					Name = manifest.Name,
					Kind = manifest.Kind,
				};
				VerifySbom(sbom, packageManifest, releaseManifestPath, packagePath, checksumErrors);

				var checksumEntries = ReadChecksums(checksumsPath);
				VerifyChecksum(packagePath, checksumEntries, checksumErrors);
				VerifyChecksum(releaseManifestPath, checksumEntries, checksumErrors);
				VerifyChecksum(provenancePath, checksumEntries, checksumErrors);
				VerifyChecksum(sbomPath, checksumEntries, checksumErrors);
				VerifyChecksum(releaseNotesPath, checksumEntries, checksumErrors);

				if (!string.Equals(ComputeSha256(packagePath), manifest.ArchiveSha256, StringComparison.OrdinalIgnoreCase))
				{
					checksumErrors.Add("ArchiveSha256 in release manifest does not match the package file hash.");
				}

				return new PacxPackageReleaseVerificationResult(missingFiles, checksumErrors);
			}
			catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or InvalidDataException or JsonException)
			{
				return new PacxPackageReleaseVerificationResult(Array.Empty<string>(), Array.Empty<string>(), ex);
			}
		}

		private static void VerifyProvenance(
			PacxPackageReleaseProvenance provenance,
			PacxPackageReleaseManifest manifest,
			string releaseManifestPath,
			string packagePath,
			ICollection<string> checksumErrors)
		{
			if (!string.Equals(provenance.Version, manifest.Version, StringComparison.OrdinalIgnoreCase))
			{
				checksumErrors.Add("Provenance version does not match the release manifest.");
			}

			if (!string.Equals(Path.GetFileName(provenance.ArchivePath), Path.GetFileName(packagePath), StringComparison.OrdinalIgnoreCase))
			{
				checksumErrors.Add("Provenance archive path does not match the staged package file.");
			}

			if (!string.Equals(Path.GetFileName(provenance.ManifestPath), Path.GetFileName(releaseManifestPath), StringComparison.OrdinalIgnoreCase))
			{
				checksumErrors.Add("Provenance manifest path does not match the staged release manifest.");
			}

			if (!string.Equals(provenance.ManifestSha256, ComputeSha256(releaseManifestPath), StringComparison.OrdinalIgnoreCase))
			{
				checksumErrors.Add("Provenance manifest hash does not match the staged release manifest hash.");
			}
		}

		private static void VerifySbom(
			PacxPackageReleaseSbom sbom,
			PacxPackageManifest manifest,
			string releaseManifestPath,
			string packagePath,
			ICollection<string> checksumErrors)
		{
			if (!string.Equals(sbom.Version, manifest.Version, StringComparison.OrdinalIgnoreCase))
			{
				checksumErrors.Add("SBOM version does not match the release manifest.");
			}

			if (!string.Equals(sbom.PackageKind, manifest.Kind, StringComparison.OrdinalIgnoreCase))
			{
				checksumErrors.Add("SBOM package kind does not match the release manifest.");
			}

			if (!string.Equals(Path.GetFileName(sbom.PackagePath), Path.GetFileName(packagePath), StringComparison.OrdinalIgnoreCase))
			{
				checksumErrors.Add("SBOM package path does not match the staged package file.");
			}

			VerifySbomComponents(sbom.Components, manifest, releaseManifestPath, packagePath, checksumErrors);
		}

		private static void VerifySbomComponents(
			IReadOnlyList<PacxPackageSbomComponent> components,
			PacxPackageManifest manifest,
			string releaseManifestPath,
			string packagePath,
			ICollection<string> checksumErrors)
		{
			if (components.Count < 2)
			{
				checksumErrors.Add("SBOM does not list the archive and manifest components.");
				return;
			}

			var hasArchiveComponent = components.Any(component => string.Equals(component.Type, "archive", StringComparison.OrdinalIgnoreCase));
			var hasManifestComponent = components.Any(component => string.Equals(component.Type, "manifest", StringComparison.OrdinalIgnoreCase));
			if (!hasArchiveComponent)
			{
				checksumErrors.Add("SBOM does not include the archive component.");
			}

			if (!hasManifestComponent)
			{
				checksumErrors.Add("SBOM does not include the manifest component.");
			}

			var archiveComponent = components.FirstOrDefault(component => string.Equals(component.Type, "archive", StringComparison.OrdinalIgnoreCase));
			if (archiveComponent is not null)
			{
				if (!string.Equals(Path.GetFileName(archiveComponent.Path), Path.GetFileName(packagePath), StringComparison.OrdinalIgnoreCase))
				{
					checksumErrors.Add("SBOM archive component path does not match the staged package file.");
				}

				if (!string.Equals(archiveComponent.Hash ?? string.Empty, ComputeSha256(packagePath), StringComparison.OrdinalIgnoreCase))
				{
					checksumErrors.Add("SBOM archive component hash does not match the staged package file hash.");
				}
			}

			var manifestComponent = components.FirstOrDefault(component => string.Equals(component.Type, "manifest", StringComparison.OrdinalIgnoreCase));
			if (manifestComponent is not null)
			{
				if (!string.Equals(Path.GetFileName(manifestComponent.Path), Path.GetFileName(releaseManifestPath), StringComparison.OrdinalIgnoreCase))
				{
					checksumErrors.Add("SBOM manifest component path does not match the staged release manifest.");
				}

				if (!string.Equals(manifestComponent.Hash ?? string.Empty, ComputeSha256(releaseManifestPath), StringComparison.OrdinalIgnoreCase))
				{
					checksumErrors.Add("SBOM manifest component hash does not match the staged release manifest hash.");
				}
			}

			foreach (var artifact in manifest.Artifacts)
			{
				var component = components.FirstOrDefault(entry => string.Equals(entry.Path, artifact.Path, StringComparison.OrdinalIgnoreCase));
				if (component is null)
				{
					checksumErrors.Add($"SBOM does not include artifact component <{artifact.Path}>.");
					continue;
				}

				if (!string.Equals(component.Type, artifact.Role, StringComparison.OrdinalIgnoreCase))
				{
					checksumErrors.Add($"SBOM component type does not match artifact role for <{artifact.Path}>.");
				}

				if (!string.Equals(component.Hash ?? string.Empty, artifact.Sha256 ?? string.Empty, StringComparison.OrdinalIgnoreCase))
				{
					checksumErrors.Add($"SBOM component hash does not match artifact hash for <{artifact.Path}>.");
				}
			}
		}

		private static Dictionary<string, string> ReadChecksums(string checksumsPath)
		{
			var entries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			foreach (var line in File.ReadAllLines(checksumsPath))
			{
				if (string.IsNullOrWhiteSpace(line))
				{
					continue;
				}

				var parts = line.Split(new[] { "  " }, StringSplitOptions.None);
				if (parts.Length != 2)
				{
					continue;
				}

				entries[parts[1].Trim()] = parts[0].Trim();
			}

			return entries;
		}

		private static void VerifyChecksum(string path, IReadOnlyDictionary<string, string> checksumEntries, ICollection<string> checksumErrors)
		{
			var fileName = Path.GetFileName(path);
			if (!checksumEntries.TryGetValue(fileName, out var expectedHash))
			{
				checksumErrors.Add($"Missing checksum entry for {fileName}.");
				return;
			}

			var actualHash = ComputeSha256(path);
			if (!string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase))
			{
				checksumErrors.Add($"Checksum mismatch for {fileName}.");
			}
		}

		private static string ComputeSha256(string path)
		{
			using var stream = File.OpenRead(path);
			var hash = SHA256.HashData(stream);
			return Convert.ToHexString(hash).ToLowerInvariant();
		}
	}
}
