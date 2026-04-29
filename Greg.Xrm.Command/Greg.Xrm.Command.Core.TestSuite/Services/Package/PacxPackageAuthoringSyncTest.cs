using System.Linq;
using Greg.Xrm.Command.Commands.Package;
using Greg.Xrm.Command.TestSuite;

namespace Greg.Xrm.Command.Services.Package
{
	[TestClass]
	public class PacxPackageAuthoringSyncTest
	{
		[TestMethod]
		public async Task SyncShouldAddMissingArtifactsAndDeploymentSteps()
		{
			var root = TestTempPath.CreateDirectory("pacx_sync");

			try
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
				Directory.CreateDirectory(Path.Combine(root, "data"));
				Directory.CreateDirectory(Path.Combine(root, "scripts"));
				File.WriteAllBytes(Path.Combine(root, "payload", "solution.zip"), new byte[] { 1, 2, 3 });
				File.WriteAllText(Path.Combine(root, "data", "accounts.json"), "[]");
				File.WriteAllText(Path.Combine(root, "scripts", "cleanup.js"), "console.log('ok');");
				File.WriteAllText(Path.Combine(root, "README.md"), "ignored");

				var authoring = new PacxPackageAuthoringService();
				var result = await authoring.SyncAsync(new PackageSyncCommand
				{
					Path = root
				}, CancellationToken.None);

				Assert.AreEqual(3, result.AddedArtifacts);
				Assert.AreEqual(2, result.AddedSteps);
				Assert.AreEqual(0, result.PrunedArtifacts);
				Assert.AreEqual(0, result.PrunedSteps);

				using var package = new PacxPackageReader().Open(root);
				Assert.AreEqual(3, package.Manifest.Artifacts.Count);
				Assert.AreEqual(2, package.Manifest.Deployment.Count);
				Assert.IsTrue(package.Manifest.Artifacts.Any(x => x.Path == "scripts/cleanup.js"));
				Assert.IsFalse(package.Manifest.Artifacts.Any(x => x.Path == "README.md"));
			}
			finally
			{
				if (Directory.Exists(root))
				{
					Directory.Delete(root, true);
				}
			}
		}

		[TestMethod]
		public async Task SyncWithPruneMissingShouldRemoveStaleManifestEntries()
		{
			var root = TestTempPath.CreateDirectory("pacx_sync_prune");
			var solutionSource = TestTempPath.CreateFilePath("pacx_solution", ".zip");
			var dataSource = TestTempPath.CreateFilePath("pacx_data", ".json");

			try
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

				await File.WriteAllBytesAsync(solutionSource, new byte[] { 1, 2, 3, 4 });
				await File.WriteAllTextAsync(dataSource, "[]");

				var authoring = new PacxPackageAuthoringService();
				await authoring.AddSolutionAsync(new PackageAddSolutionCommand
				{
					Path = root,
					SourcePath = solutionSource,
					ArtifactPath = "payload/solution.zip",
					Force = true
				}, CancellationToken.None);
				await authoring.AddDataAsync(new PackageAddDataCommand
				{
					Path = root,
					SourcePath = dataSource,
					ArtifactPath = "data/accounts.json",
					Table = "account",
					Force = true
				}, CancellationToken.None);

				File.Delete(Path.Combine(root, "data", "accounts.json"));

				var result = await authoring.SyncAsync(new PackageSyncCommand
				{
					Path = root,
					PruneMissing = true
				}, CancellationToken.None);

				Assert.AreEqual(0, result.AddedArtifacts);
				Assert.AreEqual(0, result.AddedSteps);
				Assert.AreEqual(1, result.PrunedArtifacts);
				Assert.AreEqual(1, result.PrunedSteps);

				using var package = new PacxPackageReader().Open(root);
				Assert.AreEqual(1, package.Manifest.Artifacts.Count);
				Assert.AreEqual(1, package.Manifest.Deployment.Count);
				Assert.AreEqual("payload/solution.zip", package.Manifest.Artifacts[0].Path);
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
		public async Task SyncShouldNormalizeSolutionOnlyPackagesToSolutionKind()
		{
			var root = TestTempPath.CreateDirectory("pacx_sync_solution_kind");
			var solutionSource = TestTempPath.CreateFilePath("pacx_solution_only", ".zip");

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

				var result = await authoring.SyncAsync(new PackageSyncCommand
				{
					Path = root
				}, CancellationToken.None);

				Assert.AreEqual(0, result.AddedArtifacts);
				Assert.AreEqual(0, result.AddedSteps);

				using var package = new PacxPackageReader().Open(root);
				Assert.AreEqual("solution", package.Manifest.Kind);
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
	}
}
