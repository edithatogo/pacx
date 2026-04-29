using System.Threading;
using System.Text.Json;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Package;

namespace Greg.Xrm.Command.Commands.Package
{
	[TestClass]
	public class PackageListCommandExecutorTest
	{
		[TestMethod]
		public async Task ExecuteShouldShowArtifactsAndStatus()
		{
			var tempDir = TestTempPath.CreateDirectory("pacx_package_list");
			try
			{
				WriteSamplePackage(tempDir);

				var output = new OutputToMemory();
				var executor = new PackageListCommandExecutor(output, new PacxPackageReader());

				var result = await executor.ExecuteAsync(new PackageListCommand { Path = tempDir }, CancellationToken.None);

				Assert.IsTrue(result.IsSuccess);
				var text = output.ToString();
				StringAssert.Contains(text, "PACX package list: Contoso Sample");
				StringAssert.Contains(text, "Kind: bundle (mixed or release package)");
				StringAssert.Contains(text, "Contract: No additional kind-specific constraints.");
				StringAssert.Contains(text, "payload/solution.zip");
				StringAssert.Contains(text, "ready");
				StringAssert.Contains(text, "data/accounts.json");
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}

		[TestMethod]
		public async Task ExecuteShouldMarkMissingArtifacts()
		{
			var tempDir = TestTempPath.CreateDirectory("pacx_package_list_missing");
			try
			{
				WriteSamplePackage(tempDir, includeDataFile: false);

				var output = new OutputToMemory();
				var executor = new PackageListCommandExecutor(output, new PacxPackageReader());

				var result = await executor.ExecuteAsync(new PackageListCommand { Path = tempDir }, CancellationToken.None);

				Assert.IsTrue(result.IsSuccess);
				StringAssert.Contains(output.ToString(), "missing");
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}

		private static void WriteSamplePackage(string rootDir, bool includeDataFile = true)
		{
			var payloadDir = Path.Combine(rootDir, "payload");
			var dataDir = Path.Combine(rootDir, "data");
			Directory.CreateDirectory(payloadDir);
			Directory.CreateDirectory(dataDir);

			File.WriteAllBytes(Path.Combine(payloadDir, "solution.zip"), new byte[] { 1, 2, 3, 4 });
			if (includeDataFile)
			{
				File.WriteAllText(Path.Combine(dataDir, "accounts.json"), "[]");
			}

			var manifest = new PacxPackageManifest
			{
				PackageId = "contoso.sample",
				Version = "1.0.0",
				Name = "Contoso Sample",
				Description = "Sample PACX package",
				Kind = "bundle",
				Artifacts =
				[
					new PacxPackageArtifact { Path = "payload/solution.zip", Role = "solution" },
					new PacxPackageArtifact { Path = "data/accounts.json", Role = "data", Required = false }
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
