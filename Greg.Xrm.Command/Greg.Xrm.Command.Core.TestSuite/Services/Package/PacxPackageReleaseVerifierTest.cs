using Greg.Xrm.Command.Commands.Package;

namespace Greg.Xrm.Command.Services.Package
{
	[TestClass]
	public class PacxPackageReleaseVerifierTest
	{
		[TestMethod]
		public async Task VerifyShouldPassForStagedReleaseFolder()
		{
			var root = TestTempPath.CreateDirectory("pacx_release_verify");
			var destination = TestTempPath.CreateDirectory("pacx_release_verify_out");

			try
			{
				await StageSampleRelease(root, destination);

				var releaseDir = Directory.GetDirectories(destination, "contoso.sample.*").Single();
				var verifier = new PacxPackageReleaseVerifier();

				var result = verifier.Verify(releaseDir);

				Assert.IsTrue(result.IsValid, $"Missing={string.Join(", ", result.MissingFiles)}; Issues={string.Join(", ", result.ChecksumErrors)}");
			}
			finally
			{
				if (Directory.Exists(root))
				{
					Directory.Delete(root, true);
				}

				if (Directory.Exists(destination))
				{
					Directory.Delete(destination, true);
				}
			}
		}

		[TestMethod]
		public async Task VerifyShouldReportMissingFiles()
		{
			var root = TestTempPath.CreateDirectory("pacx_release_verify_missing");
			var destination = TestTempPath.CreateDirectory("pacx_release_verify_missing_out");

			try
			{
				await StageSampleRelease(root, destination);

				var releaseDir = Directory.GetDirectories(destination, "contoso.sample.*").Single();
				File.Delete(Path.Combine(releaseDir, "checksums.txt"));

				var verifier = new PacxPackageReleaseVerifier();

				var result = verifier.Verify(releaseDir);

				Assert.IsFalse(result.IsValid);
				Assert.IsTrue(result.MissingFiles.Contains("checksums.txt"));
			}
			finally
			{
				if (Directory.Exists(root))
				{
					Directory.Delete(root, true);
				}

				if (Directory.Exists(destination))
				{
					Directory.Delete(destination, true);
				}
			}
		}

		[TestMethod]
		public async Task VerifyShouldReportMissingProvenanceAndSbom()
		{
			var root = TestTempPath.CreateDirectory("pacx_release_verify_missing_prov_sbom");
			var destination = TestTempPath.CreateDirectory("pacx_release_verify_missing_prov_sbom_out");

			try
			{
				await StageSampleRelease(root, destination);

				var releaseDir = Directory.GetDirectories(destination, "contoso.sample.*").Single();
				File.Delete(Path.Combine(releaseDir, "provenance.json"));
				File.Delete(Path.Combine(releaseDir, "sbom.json"));

				var verifier = new PacxPackageReleaseVerifier();

				var result = verifier.Verify(releaseDir);

				Assert.IsFalse(result.IsValid);
				Assert.IsTrue(result.MissingFiles.Contains("provenance.json"));
				Assert.IsTrue(result.MissingFiles.Contains("sbom.json"));
			}
			finally
			{
				if (Directory.Exists(root))
				{
					Directory.Delete(root, true);
				}

				if (Directory.Exists(destination))
				{
					Directory.Delete(destination, true);
				}
			}
		}

		[TestMethod]
		public async Task VerifyShouldReportProvenanceAndSbomDrift()
		{
			var root = TestTempPath.CreateDirectory("pacx_release_verify_drift");
			var destination = TestTempPath.CreateDirectory("pacx_release_verify_drift_out");

			try
			{
				await StageSampleRelease(root, destination);

				var releaseDir = Directory.GetDirectories(destination, "contoso.sample.*").Single();

				var provenancePath = Path.Combine(releaseDir, "provenance.json");
				var provenance = JsonSerializer.Deserialize<PacxPackageReleaseProvenance>(File.ReadAllText(provenancePath));
				Assert.IsNotNull(provenance);
				provenance!.Version = "9.9.9";
				File.WriteAllText(provenancePath, JsonSerializer.Serialize(provenance));

				var sbomPath = Path.Combine(releaseDir, "sbom.json");
				var sbom = JsonSerializer.Deserialize<PacxPackageReleaseSbom>(File.ReadAllText(sbomPath));
				Assert.IsNotNull(sbom);
				Assert.IsTrue(sbom!.Components.Count > 0);
				sbom.Components[0].Hash = "deadbeef";
				sbom!.PackageKind = "changed";
				File.WriteAllText(sbomPath, JsonSerializer.Serialize(sbom));

				var verifier = new PacxPackageReleaseVerifier();
				var result = verifier.Verify(releaseDir);

				Assert.IsFalse(result.IsValid);
				Assert.IsTrue(result.ChecksumErrors.Any(x => x.Contains("SBOM archive component hash does not match", StringComparison.OrdinalIgnoreCase)));
				Assert.IsTrue(result.ChecksumErrors.Any(x => x.Contains("Provenance version does not match", StringComparison.OrdinalIgnoreCase)));
				Assert.IsTrue(result.ChecksumErrors.Any(x => x.Contains("SBOM package kind does not match", StringComparison.OrdinalIgnoreCase)));
			}
			finally
			{
				if (Directory.Exists(root))
				{
					Directory.Delete(root, true);
				}

				if (Directory.Exists(destination))
				{
					Directory.Delete(destination, true);
				}
			}
		}

		private static async Task StageSampleRelease(string root, string destination)
		{
			var initializer = new PacxPackageInitializer();
			await initializer.InitializeAsync(new PackageInitCommand
			{
				Path = root,
				PackageId = "contoso.sample",
				Version = "1.0.0",
				Name = "Contoso Sample",
				Force = true
			}, CancellationToken.None);

			Directory.CreateDirectory(Path.Combine(root, "payload"));
			File.WriteAllBytes(Path.Combine(root, "payload", "solution.zip"), new byte[] { 1, 2, 3, 4 });

			var authoring = new PacxPackageAuthoringService();
			await authoring.SyncAsync(new PackageSyncCommand { Path = root }, CancellationToken.None);

			var releaser = new PacxPackageReleaser(new PacxPackageReader(), new PacxPackagePublisher(new PacxPackageReader(), new PacxPackageBuilder(new PacxPackageReader())));
			await releaser.ReleaseAsync(new PackageReleaseCommand
			{
				Path = root,
				DestinationPath = destination,
				Version = "2.0.0"
			}, CancellationToken.None);
		}
	}
}
