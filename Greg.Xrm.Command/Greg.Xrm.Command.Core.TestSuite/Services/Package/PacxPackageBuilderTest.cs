using Greg.Xrm.Command.Services.Package;

namespace Greg.Xrm.Command.Services.Package
{
	[TestClass]
	public class PacxPackageBuilderTest
	{
		[TestMethod]
		public void BuildFromDirectoryShouldCreateArchiveThatCanBeReadBack()
		{
			var root = TestTempPath.CreateDirectory("pacx_build");
			WriteSamplePackage(root);

			var output = TestTempPath.CreateFilePath("pacx_build", ".pacx");
			var reader = new PacxPackageReader();
			var builder = new PacxPackageBuilder(reader);

			var builtPath = builder.BuildAsync(root, output, CancellationToken.None).GetAwaiter().GetResult();

			Assert.AreEqual(Path.GetFullPath(output), builtPath);
			Assert.IsTrue(File.Exists(builtPath));

			using var package = reader.Open(builtPath);
			Assert.AreEqual("contoso.sample", package.Manifest.PackageId);
			Assert.IsTrue(package.Exists("payload/solution.zip"));
		}

		[TestMethod]
		public async Task BuildSolutionStarterShouldRoundTrip()
		{
			var root = TestTempPath.CreateDirectory("pacx_build_solution");
			var initializer = new PacxPackageInitializer();
			await initializer.InitializeAsync(new Commands.Package.PackageInitCommand
			{
				Path = root,
				PackageId = "contoso.solution",
				Version = "1.0.0",
				Name = "Contoso Solution",
				Kind = "solution",
				Force = true
			}, CancellationToken.None);

			var output = TestTempPath.CreateFilePath("pacx_build_solution", ".pacx");
			var reader = new PacxPackageReader();
			var builder = new PacxPackageBuilder(reader);

			var builtPath = await builder.BuildAsync(root, output, CancellationToken.None);

			using var package = reader.Open(builtPath);
			Assert.AreEqual("solution", package.Manifest.Kind);
			Assert.IsTrue(package.Exists("payload/solution.zip"));
			Assert.AreEqual(1, package.Manifest.Artifacts.Count);
			Assert.AreEqual(1, package.Manifest.Deployment.Count);
		}

		[TestMethod]
		public async Task BuildDataStarterShouldRoundTrip()
		{
			var root = TestTempPath.CreateDirectory("pacx_build_data");
			var initializer = new PacxPackageInitializer();
			await initializer.InitializeAsync(new Commands.Package.PackageInitCommand
			{
				Path = root,
				PackageId = "contoso.data",
				Version = "1.0.0",
				Name = "Contoso Data",
				Kind = "data",
				Force = true
			}, CancellationToken.None);

			var output = TestTempPath.CreateFilePath("pacx_build_data", ".pacx");
			var reader = new PacxPackageReader();
			var builder = new PacxPackageBuilder(reader);

			var builtPath = await builder.BuildAsync(root, output, CancellationToken.None);

			using var package = reader.Open(builtPath);
			Assert.AreEqual("data", package.Manifest.Kind);
			Assert.IsTrue(package.Exists("data/records.json"));
			Assert.AreEqual(1, package.Manifest.Artifacts.Count);
			Assert.AreEqual(1, package.Manifest.Deployment.Count);
		}

		private static void WriteSamplePackage(string rootDir)
		{
			var payloadDir = Path.Combine(rootDir, "payload");
			var dataDir = Path.Combine(rootDir, "data");
			Directory.CreateDirectory(payloadDir);
			Directory.CreateDirectory(dataDir);

			File.WriteAllBytes(Path.Combine(payloadDir, "solution.zip"), new byte[] { 1, 2, 3, 4 });
			File.WriteAllText(Path.Combine(dataDir, "accounts.json"), "[]");
			File.WriteAllText(Path.Combine(rootDir, PacxPackageManifest.FileName), """
			{
			  "schemaVersion": 1,
			  "packageId": "contoso.sample",
			  "version": "1.0.0",
			  "name": "Contoso Sample",
			  "kind": "bundle",
			  "artifacts": [
				{ "path": "payload/solution.zip", "role": "solution" },
				{ "path": "data/accounts.json", "role": "data" }
			  ],
			  "deployment": [
				{ "type": "solutionImport", "artifact": "payload/solution.zip" },
				{ "type": "dataImport", "artifact": "data/accounts.json", "table": "account", "mode": "upsert" }
			  ]
			}
			""");
		}
	}
}
