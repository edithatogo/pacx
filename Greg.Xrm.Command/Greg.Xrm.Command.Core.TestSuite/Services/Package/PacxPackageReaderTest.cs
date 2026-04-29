using System.IO.Compression;
using System.Text.Json;
using Greg.Xrm.Command.Services.Package;

namespace Greg.Xrm.Command.Services.Package
{
	[TestClass]
	public class PacxPackageReaderTest
	{
		[TestMethod]
		public void OpenDirectoryPackageShouldReadManifestAndArtifacts()
		{
			var tempDir = TestTempPath.CreateDirectory("pacx_package");
			try
			{
				WriteSamplePackage(tempDir, "bundle");

				var reader = new PacxPackageReader();
				using var package = reader.Open(tempDir);

				Assert.AreEqual("contoso.sample", package.Manifest.PackageId);
				Assert.AreEqual(2, package.Entries.Count);
				Assert.IsTrue(package.Exists("payload/solution.zip"));
				Assert.IsTrue(package.Exists("data/accounts.json"));
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}

		[TestMethod]
		public void OpenZipPackageShouldReadManifestAndArtifacts()
		{
			var tempDir = TestTempPath.CreateDirectory("pacx_package");
			try
			{
				WriteSamplePackage(tempDir, "bundle");
				var zipPath = TestTempPath.CreateFilePath("pacx_package", ".pacx");
				ZipFile.CreateFromDirectory(tempDir, zipPath);

				var reader = new PacxPackageReader();
				using (var package = reader.Open(zipPath))
				{
					Assert.AreEqual("contoso.sample", package.Manifest.PackageId);
					Assert.AreEqual(2, package.Entries.Count);
					Assert.IsTrue(package.Exists("payload/solution.zip"));
					Assert.IsTrue(package.Exists("data/accounts.json"));
				}
				File.Delete(zipPath);
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}

		[TestMethod]
		public void OpenSolutionPackageShouldValidateKindRules()
		{
			var tempDir = TestTempPath.CreateDirectory("pacx_solution_package");
			try
			{
				WriteSamplePackage(tempDir, "solution");

				var reader = new PacxPackageReader();
				using var package = reader.Open(tempDir);

				Assert.AreEqual("solution", package.Manifest.Kind);
				Assert.AreEqual(2, package.Entries.Count);
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}

		[TestMethod]
		public void OpenPackageWithInvalidKindShouldFail()
		{
			var tempDir = TestTempPath.CreateDirectory("pacx_invalid_kind");
			try
			{
				WriteSamplePackage(tempDir, "bundle");

				var manifest = JsonSerializer.Deserialize<PacxPackageManifest>(File.ReadAllText(Path.Combine(tempDir, PacxPackageManifest.FileName)))!;
				manifest.Kind = "invalid-kind";
				File.WriteAllText(Path.Combine(tempDir, PacxPackageManifest.FileName), JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }));

				var reader = new PacxPackageReader();
				Assert.Throws<InvalidDataException>(() => reader.Open(tempDir));
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}

		private static void WriteSamplePackage(string rootDir, string kind)
		{
			var payloadDir = Path.Combine(rootDir, "payload");
			var dataDir = Path.Combine(rootDir, "data");
			Directory.CreateDirectory(payloadDir);
			Directory.CreateDirectory(dataDir);

			File.WriteAllBytes(Path.Combine(payloadDir, "solution.zip"), new byte[] { 1, 2, 3, 4 });
			File.WriteAllText(Path.Combine(dataDir, "accounts.json"), "[]");

			var manifest = new PacxPackageManifest
			{
				PackageId = "contoso.sample",
				Version = "1.0.0",
				Name = "Contoso Sample",
				Description = "Sample PACX package",
				Kind = kind,
				Artifacts =
				[
					new PacxPackageArtifact { Path = "payload/solution.zip", Role = "solution" },
					new PacxPackageArtifact { Path = "data/accounts.json", Role = "data" }
				],
				Deployment =
				[
					new PacxPackageDeploymentStep { Type = "solutionImport", Artifact = "payload/solution.zip" },
					new PacxPackageDeploymentStep { Type = "dataImport", Artifact = "data/accounts.json", Table = "account", Mode = "upsert" }
				]
			};

			var json = JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true });
			File.WriteAllText(Path.Combine(rootDir, PacxPackageManifest.FileName), json);
		}
	}
}
