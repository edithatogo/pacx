using Greg.Xrm.Command.Commands.Package;
using Greg.Xrm.Command.Services.Package;
using Greg.Xrm.Command.TestSuite;

namespace Greg.Xrm.Command.Services.Package
{
	[TestClass]
	public class PacxPackageAuthoringRemoveTest
	{
		[TestMethod]
		public async Task RemoveShouldDeleteArtifactsAndManifestEntries()
		{
			var root = TestTempPath.CreateDirectory("pacx_remove");
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
					Table = "account",
					ArtifactPath = "data/accounts.json",
					Force = true
				}, CancellationToken.None);

				await authoring.RemoveDataAsync(new PackageRemoveDataCommand
				{
					Path = root,
					ArtifactPath = "data/accounts.json"
				}, CancellationToken.None);
				await authoring.RemoveSolutionAsync(new PackageRemoveSolutionCommand
				{
					Path = root,
					ArtifactPath = "payload/solution.zip"
				}, CancellationToken.None);

				using var package = new PacxPackageReader().Open(root);
				Assert.AreEqual(0, package.Manifest.Artifacts.Count);
				Assert.AreEqual(0, package.Manifest.Deployment.Count);
				Assert.IsFalse(File.Exists(Path.Combine(root, "payload", "solution.zip")));
				Assert.IsFalse(File.Exists(Path.Combine(root, "data", "accounts.json")));
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
		public async Task RemoveWithForceShouldDropManifestEntriesEvenIfFileIsMissing()
		{
			var root = TestTempPath.CreateDirectory("pacx_remove_force");
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

				File.Delete(Path.Combine(root, "payload", "solution.zip"));

				await authoring.RemoveSolutionAsync(new PackageRemoveSolutionCommand
				{
					Path = root,
					ArtifactPath = "payload/solution.zip",
					Force = true
				}, CancellationToken.None);

				using var package = new PacxPackageReader().Open(root);
				Assert.AreEqual(0, package.Manifest.Artifacts.Count);
				Assert.AreEqual(0, package.Manifest.Deployment.Count);
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
		public async Task RemoveLastManagedArtifactShouldNormalizeKindToBundle()
		{
			var root = TestTempPath.CreateDirectory("pacx_remove_kind");
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
					Force = true
				}, CancellationToken.None);

				await authoring.RemoveDataAsync(new PackageRemoveDataCommand
				{
					Path = root,
					ArtifactPath = "data/accounts.json"
				}, CancellationToken.None);

				using var package = new PacxPackageReader().Open(root);
				Assert.AreEqual("bundle", package.Manifest.Kind);
				Assert.AreEqual(0, package.Manifest.Artifacts.Count);
				Assert.AreEqual(0, package.Manifest.Deployment.Count);
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
