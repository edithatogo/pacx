using System.IO.Compression;
using System.Text.Json;

namespace Greg.Xrm.Command.Services.Package
{
	public sealed class PacxPackageReader : IPacxPackageReader
	{
		private static readonly JsonSerializerOptions serializerOptions = new()
		{
			PropertyNameCaseInsensitive = true
		};

		public IPacxPackageSource Open(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				throw new ArgumentException("Package path not provided.", nameof(path));
			}

			if (Directory.Exists(path))
			{
				return new PacxDirectoryPackageSource(path, serializerOptions);
			}

			if (!File.Exists(path))
			{
				throw new FileNotFoundException($"Package path <{path}> does not exist.", path);
			}

			return new PacxZipPackageSource(path, serializerOptions);
		}
	}

	internal sealed class PacxDirectoryPackageSource : IPacxPackageSource
	{
		private readonly string rootPath;
		private readonly JsonSerializerOptions serializerOptions;

		public PacxDirectoryPackageSource(string rootPath, JsonSerializerOptions serializerOptions)
		{
			this.rootPath = Path.GetFullPath(rootPath);
			this.serializerOptions = serializerOptions;
			this.Manifest = ReadManifest();
			this.Entries = EnumerateEntries().ToArray();
			ValidateArtifacts();
		}

		public string SourcePath => this.rootPath;

		public PacxPackageManifest Manifest { get; }

		public IReadOnlyList<PacxPackageEntry> Entries { get; }

		public bool Exists(string relativePath)
		{
			return File.Exists(GetFullPath(relativePath));
		}

		public Stream OpenRead(string relativePath)
		{
			var fullPath = GetFullPath(relativePath);
			if (!File.Exists(fullPath))
			{
				throw new FileNotFoundException($"Artifact <{relativePath}> does not exist in package <{this.rootPath}>.", fullPath);
			}

			return File.OpenRead(fullPath);
		}

		public void Dispose()
		{
		}

		private PacxPackageManifest ReadManifest()
		{
			var manifestPath = GetFullPath(PacxPackageManifest.FileName);
			if (!File.Exists(manifestPath))
			{
				throw new FileNotFoundException($"Manifest file <{PacxPackageManifest.FileName}> not found in package directory <{this.rootPath}>.", manifestPath);
			}

			var json = File.ReadAllText(manifestPath);
			var manifest = JsonSerializer.Deserialize<PacxPackageManifest>(json, this.serializerOptions);
			if (manifest == null)
			{
				throw new InvalidDataException($"Manifest file <{manifestPath}> could not be parsed.");
			}

			PacxPackageReaderValidation.ValidateManifest(manifest, this.rootPath);
			return manifest;
		}

		private IEnumerable<PacxPackageEntry> EnumerateEntries()
		{
			foreach (var file in Directory.EnumerateFiles(this.rootPath, "*", SearchOption.AllDirectories))
			{
				var relativePath = Path.GetRelativePath(this.rootPath, file).Replace('\\', '/');
				if (string.Equals(relativePath, PacxPackageManifest.FileName, StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}

				yield return new PacxPackageEntry
				{
					Path = relativePath,
					Length = new FileInfo(file).Length
				};
			}
		}

		private string GetFullPath(string relativePath)
		{
			var normalized = PacxPackagePath.NormalizePath(relativePath);
			return Path.Combine(this.rootPath, normalized.Replace('/', Path.DirectorySeparatorChar));
		}

		private void ValidateArtifacts()
		{
			foreach (var artifact in this.Manifest.Artifacts.Where(x => x.Required))
			{
				if (!Exists(artifact.Path))
				{
					throw new FileNotFoundException($"Required artifact <{artifact.Path}> is missing from package <{this.rootPath}>.");
				}
			}
		}
	}

	internal sealed class PacxZipPackageSource : IPacxPackageSource
	{
		private readonly ZipArchive archive;
		private readonly JsonSerializerOptions serializerOptions;

		public PacxZipPackageSource(string sourcePath, JsonSerializerOptions serializerOptions)
		{
			this.SourcePath = Path.GetFullPath(sourcePath);
			this.serializerOptions = serializerOptions;
			this.archive = ZipFile.OpenRead(this.SourcePath);
			this.Manifest = ReadManifest();
			this.Entries = EnumerateEntries().ToArray();
			ValidateArtifacts();
		}

		public string SourcePath { get; }

		public PacxPackageManifest Manifest { get; }

		public IReadOnlyList<PacxPackageEntry> Entries { get; }

		public bool Exists(string relativePath)
		{
			return FindEntry(relativePath) != null;
		}

		public Stream OpenRead(string relativePath)
		{
			var entry = FindEntry(relativePath);
			if (entry == null)
			{
				throw new FileNotFoundException($"Artifact <{relativePath}> does not exist in package <{this.SourcePath}>.");
			}

			return entry.Open();
		}

		public void Dispose()
		{
			this.archive.Dispose();
		}

		private PacxPackageManifest ReadManifest()
		{
			var entry = FindEntry(PacxPackageManifest.FileName);
			if (entry == null)
			{
				throw new FileNotFoundException($"Manifest file <{PacxPackageManifest.FileName}> not found in package <{this.SourcePath}>.");
			}

			using var stream = entry.Open();
			var manifest = JsonSerializer.Deserialize<PacxPackageManifest>(stream, this.serializerOptions);
			if (manifest == null)
			{
				throw new InvalidDataException($"Manifest file <{this.SourcePath}> could not be parsed.");
			}

			PacxPackageReaderValidation.ValidateManifest(manifest, this.SourcePath);
			return manifest;
		}

		private IEnumerable<PacxPackageEntry> EnumerateEntries()
		{
			foreach (var entry in this.archive.Entries)
			{
				if (string.IsNullOrWhiteSpace(entry.FullName) || entry.FullName.EndsWith("/", StringComparison.Ordinal))
				{
					continue;
				}

				var normalized = PacxPackagePath.NormalizePath(entry.FullName);
				if (string.Equals(normalized, PacxPackageManifest.FileName, StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}

				yield return new PacxPackageEntry
				{
					Path = normalized,
					Length = entry.Length
				};
			}
		}

		private ZipArchiveEntry? FindEntry(string relativePath)
		{
			var normalized = PacxPackagePath.NormalizePath(relativePath);
			return this.archive.Entries.FirstOrDefault(entry => string.Equals(PacxPackagePath.NormalizePath(entry.FullName), normalized, StringComparison.OrdinalIgnoreCase));
		}

		private void ValidateArtifacts()
		{
			foreach (var artifact in this.Manifest.Artifacts.Where(x => x.Required))
			{
				if (!Exists(artifact.Path))
				{
					throw new FileNotFoundException($"Required artifact <{artifact.Path}> is missing from package <{this.SourcePath}>.");
				}
			}
		}
	}

	internal static class PacxPackageReaderValidation
	{
		internal static void ValidateManifest(PacxPackageManifest manifest, string sourcePath)
		{
			var errors = new List<string>();

			if (manifest.SchemaVersion < 1)
			{
				errors.Add("schemaVersion must be >= 1.");
			}

			if (string.IsNullOrWhiteSpace(manifest.PackageId))
			{
				errors.Add("packageId is required.");
			}

			if (string.IsNullOrWhiteSpace(manifest.Version))
			{
				errors.Add("version is required.");
			}

			if (string.IsNullOrWhiteSpace(manifest.Name))
			{
				errors.Add("name is required.");
			}

			if (string.IsNullOrWhiteSpace(manifest.Kind))
			{
				errors.Add("kind is required.");
			}
			else if (!PacxPackageKinds.All.Contains(manifest.Kind.Trim().ToLowerInvariant()))
			{
				errors.Add($"kind must be one of: {string.Join(", ", PacxPackageKinds.All)}.");
			}

			var normalizedKind = string.IsNullOrWhiteSpace(manifest.Kind)
				? PacxPackageKinds.Bundle
				: manifest.Kind.Trim().ToLowerInvariant();

			foreach (var artifact in manifest.Artifacts ?? [])
			{
				if (string.IsNullOrWhiteSpace(artifact.Path))
				{
					errors.Add("artifact.path is required.");
					continue;
				}

				if (string.IsNullOrWhiteSpace(artifact.Role))
				{
					errors.Add($"artifact.role is required for <{artifact.Path}>.");
				}
			}

			foreach (var step in manifest.Deployment ?? [])
			{
				if (string.IsNullOrWhiteSpace(step.Type))
				{
					errors.Add("deployment step type is required.");
				}

				if (string.IsNullOrWhiteSpace(step.Artifact))
				{
					errors.Add($"deployment step artifact is required for step <{step.Type}>.");
				}
			}

			if (normalizedKind == PacxPackageKinds.Solution)
			{
				if (!(manifest.Artifacts ?? []).Any(x => string.Equals(x.Role, PacxPackageKinds.Solution, StringComparison.OrdinalIgnoreCase)))
				{
					errors.Add("solution packages must include at least one artifact with role <solution>.");
				}

				if (!(manifest.Deployment ?? []).Any(x => string.Equals(x.Type, "solutionImport", StringComparison.OrdinalIgnoreCase)))
				{
					errors.Add("solution packages must include at least one deployment step of type <solutionImport>.");
				}
			}
			else if (normalizedKind == PacxPackageKinds.Data)
			{
				if (!(manifest.Artifacts ?? []).Any(x => string.Equals(x.Role, PacxPackageKinds.Data, StringComparison.OrdinalIgnoreCase)))
				{
					errors.Add("data packages must include at least one artifact with role <data>.");
				}

				if (!(manifest.Deployment ?? []).Any(x => string.Equals(x.Type, "dataImport", StringComparison.OrdinalIgnoreCase)))
				{
					errors.Add("data packages must include at least one deployment step of type <dataImport>.");
				}
			}

			if (errors.Count > 0)
			{
				throw new InvalidDataException($"Invalid PACX package manifest in <{sourcePath}>: {string.Join(" ", errors)}");
			}
		}
	}

	internal static class PacxPackagePath
	{
		internal static string NormalizePath(string path)
		{
			return path.Replace('\\', '/').TrimStart('/');
		}
	}
}
