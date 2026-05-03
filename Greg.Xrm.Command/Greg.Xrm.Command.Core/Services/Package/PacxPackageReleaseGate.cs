using System.Reflection;

namespace Greg.Xrm.Command.Services.Package
{
	/// <summary>
	/// Combined release gate that validates provenance, SBOM, checksums,
	/// and assembly signing before a release can be promoted.
	/// </summary>
	public interface IPacxPackageReleaseGate
	{
		PacxPackageReleaseGateResult Evaluate(string releaseDirectory);
	}

	public sealed class PacxPackageReleaseGateResult
	{
		public bool ProvenanceValid { get; init; }
		public bool SbomComplete { get; init; }
		public bool ChecksumsMatch { get; init; }
		public bool AssembliesSigned { get; init; }
		public bool HasSigstoreSignatures { get; init; }
		public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

		public bool AllGatesPass =>
			ProvenanceValid && SbomComplete && ChecksumsMatch &&
			AssembliesSigned && HasSigstoreSignatures &&
			Errors.Count == 0;
	}

	public sealed class PacxPackageReleaseGate : IPacxPackageReleaseGate
	{
		private readonly IPacxPackageReleaseVerifier _verifier;

		public PacxPackageReleaseGate(IPacxPackageReleaseVerifier verifier)
		{
			ArgumentNullException.ThrowIfNull(verifier);
			_verifier = verifier;
		}

		public PacxPackageReleaseGateResult Evaluate(string releaseDirectory)
		{
			var errors = new List<string>();
			var verifyResult = _verifier.Verify(releaseDirectory);

			if (verifyResult.Exception is not null)
			{
				errors.Add($"Verifier exception: {verifyResult.Exception.Message}");
				return FailedResult(errors);
			}

			var provenanceValid = verifyResult.IsValid && verifyResult.MissingFiles.All(f => f != "provenance.json");
			var sbomComplete = verifyResult.IsValid && verifyResult.MissingFiles.All(f => f != "sbom.json");
			var checksumsMatch = verifyResult.ChecksumErrors.Count == 0;

			if (verifyResult.MissingFiles.Count > 0)
			{
				errors.AddRange(verifyResult.MissingFiles.Select(f => $"Missing file: {f}"));
			}

			if (verifyResult.ChecksumErrors.Count > 0)
			{
				errors.AddRange(verifyResult.ChecksumErrors.Select(e => $"Checksum error: {e}"));
			}

			// Verify strong-name signing of assemblies in the release
			var assembliesSigned = CheckAssemblySigning(releaseDirectory, errors);

			// Check for Sigstore .sigstore bundle files
			var hasSigstoreSignatures = CheckSigstoreSignatures(releaseDirectory, errors);

			return new PacxPackageReleaseGateResult
			{
				ProvenanceValid = provenanceValid,
				SbomComplete = sbomComplete,
				ChecksumsMatch = checksumsMatch,
				AssembliesSigned = assembliesSigned,
				HasSigstoreSignatures = hasSigstoreSignatures,
				Errors = errors.AsReadOnly(),
			};
		}

		private static bool CheckAssemblySigning(string releaseDirectory, ICollection<string> errors)
		{
			// Look for .pacx files and their contained assemblies
			var pacxFiles = Directory.GetFiles(releaseDirectory, "*.pacx", SearchOption.TopDirectoryOnly);
			if (pacxFiles.Length == 0)
			{
				// Check for NuGet packages instead
				var nupkgFiles = Directory.GetFiles(releaseDirectory, "*.nupkg", SearchOption.TopDirectoryOnly);
				if (nupkgFiles.Length == 0)
				{
					errors.Add("No package files found for signing verification.");
					return false;
				}

				return VerifyNupkgAssemblies(nupkgFiles, errors);
			}

			return VerifyPacxAssemblies(pacxFiles, errors);
		}

		private static bool VerifyNupkgAssemblies(IReadOnlyList<string> nupkgFiles, ICollection<string> errors)
		{
			var allSigned = true;

			foreach (var nupkg in nupkgFiles)
			{
				try
				{
					using var stream = File.OpenRead(nupkg);
					using var archive = new System.IO.Compression.ZipArchive(stream, System.IO.Compression.ZipArchiveMode.Read);

					var dllEntries = archive.Entries
						.Where(e => e.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
						.Where(e => !e.Name.StartsWith("System.", StringComparison.OrdinalIgnoreCase))
						.Where(e => !e.Name.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase))
						.ToList();

					if (dllEntries.Count == 0)
					{
						continue;
					}

					foreach (var entry in dllEntries)
					{
						try
						{
							var tempDir = Path.Combine(Path.GetTempPath(), "pacx_gate_verify");
							Directory.CreateDirectory(tempDir);
							var tempPath = Path.Combine(tempDir, entry.Name);

							using (var entryStream = entry.Open())
							using (var fileStream = File.Create(tempPath))
							{
								entryStream.CopyTo(fileStream);
							}

							var assemblyName = AssemblyName.GetAssemblyName(tempPath);
							var token = assemblyName.GetPublicKeyToken();
							TryDelete(tempPath);

							if (token is null || token.Length == 0)
							{
								errors.Add($"{entry.FullName} in {nupkg} is not strong-name signed.");
								allSigned = false;
							}
						}
						catch (BadImageFormatException)
						{
							// Entry may be a native DLL or resource — skip
						}
						catch (Exception ex)
						{
							errors.Add($"Error checking {entry.FullName} in {nupkg}: {ex.Message}");
						}
					}
				}
				catch (Exception ex)
				{
					errors.Add($"Error processing {nupkg}: {ex.Message}");
					allSigned = false;
				}
			}

			return allSigned;
		}

		private static bool VerifyPacxAssemblies(IReadOnlyList<string> pacxFiles, ICollection<string> errors)
		{
			var allSigned = true;

			foreach (var pacx in pacxFiles)
			{
				try
				{
					using var stream = File.OpenRead(pacx);
					using var archive = new System.IO.Compression.ZipArchive(stream, System.IO.Compression.ZipArchiveMode.Read);

					var dllEntries = archive.Entries
						.Where(e => e.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
						.Where(e => !e.Name.StartsWith("System.", StringComparison.OrdinalIgnoreCase))
						.Where(e => !e.Name.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase))
						.ToList();

					foreach (var entry in dllEntries)
					{
						try
						{
							var tempDir = Path.Combine(Path.GetTempPath(), "pacx_gate_verify");
							Directory.CreateDirectory(tempDir);
							var tempPath = Path.Combine(tempDir, entry.Name);

							using (var entryStream = entry.Open())
							using (var fileStream = File.Create(tempPath))
							{
								entryStream.CopyTo(fileStream);
							}

							var assemblyName = AssemblyName.GetAssemblyName(tempPath);
							var token = assemblyName.GetPublicKeyToken();
							TryDelete(tempPath);

							if (token is null || token.Length == 0)
							{
								errors.Add($"{entry.FullName} in {pacx} is not strong-name signed.");
								allSigned = false;
							}
						}
						catch (BadImageFormatException)
						{
							// Skip native DLLs
						}
						catch (Exception ex)
						{
							errors.Add($"Error checking {entry.FullName} in {pacx}: {ex.Message}");
						}
					}
				}
				catch (Exception ex)
				{
					errors.Add($"Error processing {pacx}: {ex.Message}");
					allSigned = false;
				}
			}

			return allSigned;
		}

		private static bool CheckSigstoreSignatures(string releaseDirectory, ICollection<string> errors)
		{
			// Sigstore generates .sigstore bundle files alongside signed artifacts
			var sigstoreFiles = Directory.GetFiles(releaseDirectory, "*.sigstore", SearchOption.TopDirectoryOnly);

			// Also check for .intoto.jsonl (SLSA provenance attestation)
			var intotoFiles = Directory.GetFiles(releaseDirectory, "*.intoto.jsonl", SearchOption.TopDirectoryOnly);

			if (sigstoreFiles.Length == 0 && intotoFiles.Length == 0)
			{
				// Check for the attestation structure used by slsa-github-generator
				var attestationsDir = Path.Combine(releaseDirectory, "attestations");
				if (!Directory.Exists(attestationsDir) || Directory.GetFiles(attestationsDir, "*.*", SearchOption.AllDirectories).Length == 0)
				{
					errors.Add("No Sigstore .sigstore bundle files or SLSA attestations found.");
					return false;
				}
			}

			return true;
		}

		private static void TryDelete(string path)
		{
			try { File.Delete(path); } catch { /* Best-effort cleanup */ }
		}

		private static PacxPackageReleaseGateResult FailedResult(IReadOnlyList<string> errors)
		{
			return new PacxPackageReleaseGateResult
			{
				ProvenanceValid = false,
				SbomComplete = false,
				ChecksumsMatch = false,
				AssembliesSigned = false,
				HasSigstoreSignatures = false,
				Errors = errors,
			};
		}
	}
}
