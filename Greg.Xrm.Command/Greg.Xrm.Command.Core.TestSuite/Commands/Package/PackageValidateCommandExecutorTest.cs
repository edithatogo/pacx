using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Package;

namespace Greg.Xrm.Command.Commands.Package
{
	[TestClass]
	public class PackageValidateCommandExecutorTest
	{
		[TestMethod]
		public async Task ExecuteShouldShowKindDescription()
		{
			var tempDir = TestTempPath.CreateDirectory("pacx_package_validate");
			try
			{
				WriteSamplePackage(tempDir, "solution");

				var output = new OutputToMemory();
				var executor = new PackageValidateCommandExecutor(output, new PacxPackageReader());

				var result = await executor.ExecuteAsync(new PackageValidateCommand { Path = tempDir }, CancellationToken.None);

				Assert.IsTrue(result.IsSuccess);
				StringAssert.Contains(output.ToString(), "Kind: solution (solution-only deployment package)");
				StringAssert.Contains(output.ToString(), "Contract: Requires at least one solution artifact and one solutionImport step.");
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}

		private static void WriteSamplePackage(string rootDir, string kind)
		{
			var payloadDir = Path.Combine(rootDir, "payload");
			Directory.CreateDirectory(payloadDir);
			File.WriteAllBytes(Path.Combine(payloadDir, "solution.zip"), new byte[] { 1, 2, 3, 4 });

			var manifest = new PacxPackageManifest
			{
				PackageId = "contoso.sample",
				Version = "1.0.0",
				Name = "Contoso Sample",
				Kind = kind,
				Artifacts =
				[
					new PacxPackageArtifact { Path = "payload/solution.zip", Role = "solution" }
				],
				Deployment =
				[
					new PacxPackageDeploymentStep { Type = "solutionImport", Artifact = "payload/solution.zip" }
				]
			};

			File.WriteAllText(Path.Combine(rootDir, PacxPackageManifest.FileName), System.Text.Json.JsonSerializer.Serialize(manifest, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
		}
	}
}
