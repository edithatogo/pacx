using System.IO.Compression;

namespace Greg.Xrm.Command.Services.Package
{
	public sealed class PacxPackageBuilder(
		IPacxPackageReader packageReader
	) : IPacxPackageBuilder
	{
		public Task<string> BuildAsync(string sourcePath, string? outputPath, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (!Directory.Exists(sourcePath))
			{
				throw new DirectoryNotFoundException($"Package source folder <{sourcePath}> does not exist.");
			}

			using var package = packageReader.Open(sourcePath);
			var resolvedOutput = ResolveOutputPath(package.Manifest, sourcePath, outputPath);
			var fullOutput = Path.GetFullPath(resolvedOutput);
			var sourceFullPath = Path.GetFullPath(sourcePath);

			var outputDirectory = Path.GetDirectoryName(fullOutput);
			if (!string.IsNullOrWhiteSpace(outputDirectory))
			{
				Directory.CreateDirectory(outputDirectory);
			}

			var needsTempOutput = Directory.Exists(sourcePath)
				&& fullOutput.StartsWith(sourceFullPath.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
			var actualOutput = fullOutput;
			if (needsTempOutput)
			{
				var stagingDirectory = Directory.GetParent(sourceFullPath)?.FullName ?? Environment.CurrentDirectory;
				actualOutput = Path.Combine(stagingDirectory, $"{Path.GetFileNameWithoutExtension(fullOutput)}.{Guid.NewGuid():N}{Path.GetExtension(fullOutput)}");
				var stagingOutputDirectory = Path.GetDirectoryName(actualOutput);
				if (!string.IsNullOrWhiteSpace(stagingOutputDirectory))
				{
					Directory.CreateDirectory(stagingOutputDirectory);
				}
			}

			if (File.Exists(actualOutput))
			{
				File.Delete(actualOutput);
			}

			ZipFile.CreateFromDirectory(sourcePath, actualOutput, CompressionLevel.Optimal, includeBaseDirectory: false);

			if (!string.Equals(actualOutput, fullOutput, StringComparison.OrdinalIgnoreCase))
			{
				if (File.Exists(fullOutput))
				{
					File.Delete(fullOutput);
				}

				File.Move(actualOutput, fullOutput);
			}

			return Task.FromResult(fullOutput);
		}

		private static string ResolveOutputPath(PacxPackageManifest manifest, string sourcePath, string? outputPath)
		{
			if (!string.IsNullOrWhiteSpace(outputPath))
			{
				return outputPath;
			}

			var folder = Directory.GetParent(Path.GetFullPath(sourcePath))?.FullName ?? Environment.CurrentDirectory;
			var fileName = $"{manifest.PackageId}.{manifest.Version}.pacx";
			return Path.Combine(folder, fileName);
		}
	}
}
