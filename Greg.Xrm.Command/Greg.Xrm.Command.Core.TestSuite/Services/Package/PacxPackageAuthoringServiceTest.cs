using Greg.Xrm.Command.Commands.Package;
using Greg.Xrm.Command.Services.Package;
using Greg.Xrm.Command.TestSuite;

namespace Greg.Xrm.Command.Services.Package
{
	[TestClass]
	public class PacxPackageAuthoringServiceTest
	{
		[TestMethod]
		public async Task AddSolutionAndDataShouldUpdateManifestAndCopyArtifacts()
		{
			var root = TestTempPath.CreateDirectory("pacx_authoring");
			var solutionSource = TestTempPath.CreateFilePath("pacx_solution", ".zip");
			var dataSource = TestTempPath.CreateFilePath("pacx_data", ".json");

			try
			{
				await File.WriteAllBytesAsync(solutionSource, new byte[] { 1, 2, 3, 4 });
				await File.WriteAllTextAsync(dataSource, "[]");

				var initializer = new PacxPackageInitializer();
				await initializer.InitializeAsync(new PackageInitCommand
				{
					Path = root,
					PackageId = "contoso.sample",
					Version = "1.0.0",
					Name = "Contoso Sample",
					Force = true
				}, CancellationToken.None);

				var authoring = new PacxPackageAuthoringService();
				var solutionArtifact = await authoring.AddSolutionAsync(new PackageAddSolutionCommand
				{
					Path = root,
					SourcePath = solutionSource,
					ArtifactPath = "payload/solution.zip",
					Force = true
				}, CancellationToken.None);
				var dataArtifact = await authoring.AddDataAsync(new PackageAddDataCommand
				{
					Path = root,
					SourcePath = dataSource,
					Table = "account",
					ArtifactPath = "data/accounts.json",
					Mode = "upsert",
					Force = true
				}, CancellationToken.None);

				Assert.AreEqual("payload/solution.zip", solutionArtifact);
				Assert.AreEqual("data/accounts.json", dataArtifact);
				Assert.IsTrue(File.Exists(Path.Combine(root, "payload", "solution.zip")));
				Assert.IsTrue(File.Exists(Path.Combine(root, "data", "accounts.json")));

				using var package = new PacxPackageReader().Open(root);
				Assert.AreEqual("bundle", package.Manifest.Kind);
				Assert.AreEqual(2, package.Manifest.Artifacts.Count);
				Assert.AreEqual(2, package.Manifest.Deployment.Count);
				Assert.AreEqual("solutionImport", package.Manifest.Deployment[0].Type);
				Assert.AreEqual("dataImport", package.Manifest.Deployment[1].Type);
				Assert.AreEqual("account", package.Manifest.Deployment[1].Table);
			}
			finally
			{
				if (File.Exists(solutionSource))
				{
					File.Delete(solutionSource);
				}
				if (File.Exists(dataSource))
				{
					File.Delete(dataSource);
				}
				if (Directory.Exists(root))
				{
					Directory.Delete(root, true);
				}
			}
		}

		[TestMethod]
		public async Task AddSolutionShouldNormalizeKindToSolution()
		{
			var root = TestTempPath.CreateDirectory("pacx_authoring_solution");
			var solutionSource = TestTempPath.CreateFilePath("pacx_solution", ".zip");

			try
			{
				await File.WriteAllBytesAsync(solutionSource, new byte[] { 1, 2, 3, 4 });

				var initializer = new PacxPackageInitializer();
				await initializer.InitializeAsync(new PackageInitCommand
				{
					Path = root,
					PackageId = "contoso.sample",
					Version = "1.0.0",
					Name = "Contoso Sample",
					Force = true
				}, CancellationToken.None);

				var authoring = new PacxPackageAuthoringService();
				await authoring.AddSolutionAsync(new PackageAddSolutionCommand
				{
					Path = root,
					SourcePath = solutionSource,
					ArtifactPath = "payload/solution.zip",
					Force = true
				}, CancellationToken.None);

				using var package = new PacxPackageReader().Open(root);
				Assert.AreEqual("solution", package.Manifest.Kind);
				Assert.AreEqual(1, package.Manifest.Artifacts.Count);
				Assert.AreEqual(1, package.Manifest.Deployment.Count);
				Assert.AreEqual("solutionImport", package.Manifest.Deployment[0].Type);
			}
			finally
			{
				if (File.Exists(solutionSource))
				{
					File.Delete(solutionSource);
				}
				if (Directory.Exists(root))
				{
					Directory.Delete(root, true);
				}
			}
		}

		[TestMethod]
		public async Task AddDataShouldNormalizeKindToData()
		{
			var root = TestTempPath.CreateDirectory("pacx_authoring_data");
			var dataSource = TestTempPath.CreateFilePath("pacx_data", ".json");

			try
			{
				await File.WriteAllTextAsync(dataSource, "[]");

				var initializer = new PacxPackageInitializer();
				await initializer.InitializeAsync(new PackageInitCommand
				{
					Path = root,
					PackageId = "contoso.sample",
					Version = "1.0.0",
					Name = "Contoso Sample",
					Force = true
				}, CancellationToken.None);

				var authoring = new PacxPackageAuthoringService();
				await authoring.AddDataAsync(new PackageAddDataCommand
				{
					Path = root,
					SourcePath = dataSource,
					Table = "account",
					ArtifactPath = "data/accounts.json",
					Mode = "upsert",
					Force = true
				}, CancellationToken.None);

				using var package = new PacxPackageReader().Open(root);
				Assert.AreEqual("data", package.Manifest.Kind);
				Assert.AreEqual(1, package.Manifest.Artifacts.Count);
				Assert.AreEqual(1, package.Manifest.Deployment.Count);
				Assert.AreEqual("dataImport", package.Manifest.Deployment[0].Type);
				Assert.AreEqual("account", package.Manifest.Deployment[0].Table);
			}
			finally
			{
				if (File.Exists(dataSource))
				{
					File.Delete(dataSource);
				}
				if (Directory.Exists(root))
				{
					Directory.Delete(root, true);
				}
			}
		}
	}
}
