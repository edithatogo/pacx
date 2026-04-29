using Greg.Xrm.Command.Commands.Package;
using Greg.Xrm.Command.Services.Package;

namespace Greg.Xrm.Command.Services.Package
{
	[TestClass]
	public class PacxPackageInitializerTest
	{
		[TestMethod]
		public async Task InitializeShouldCreateStarterPackageStructure()
		{
			var root = TestTempPath.CreateDirectory("pacx_init");
			var initializer = new PacxPackageInitializer();
			var command = new PackageInitCommand
			{
				Path = root,
				PackageId = "contoso.sample",
				Version = "1.0.0",
				Name = "Contoso Sample",
				Description = "Sample package",
				Force = true
			};

			var result = await initializer.InitializeAsync(command, CancellationToken.None);

			Assert.AreEqual(Path.GetFullPath(root), result);
			Assert.IsTrue(File.Exists(Path.Combine(root, PacxPackageManifest.FileName)));
			Assert.IsTrue(Directory.Exists(Path.Combine(root, "payload")));
			Assert.IsTrue(Directory.Exists(Path.Combine(root, "data")));
			Assert.IsTrue(Directory.Exists(Path.Combine(root, "scripts")));

			using var package = new PacxPackageReader().Open(root);
			Assert.AreEqual("contoso.sample", package.Manifest.PackageId);
			Assert.AreEqual("1.0.0", package.Manifest.Version);
			Assert.AreEqual("bundle", package.Manifest.Kind);
			Assert.AreEqual(0, package.Manifest.Artifacts.Count);
			Assert.AreEqual(0, package.Manifest.Deployment.Count);
		}

		[TestMethod]
		public async Task InitializeShouldCreateValidSolutionStarterPackage()
		{
			var root = TestTempPath.CreateDirectory("pacx_init_solution");
			var initializer = new PacxPackageInitializer();
			var command = new PackageInitCommand
			{
				Path = root,
				PackageId = "contoso.solution",
				Version = "1.0.0",
				Name = "Contoso Solution",
				Kind = "solution",
				Force = true
			};

			await initializer.InitializeAsync(command, CancellationToken.None);

			using var package = new PacxPackageReader().Open(root);
			Assert.AreEqual("solution", package.Manifest.Kind);
			Assert.AreEqual(1, package.Manifest.Artifacts.Count);
			Assert.AreEqual(1, package.Manifest.Deployment.Count);
			Assert.IsTrue(File.Exists(Path.Combine(root, "payload", "solution.zip")));
		}

		[TestMethod]
		public async Task InitializeShouldCreateValidDataStarterPackage()
		{
			var root = TestTempPath.CreateDirectory("pacx_init_data");
			var initializer = new PacxPackageInitializer();
			var command = new PackageInitCommand
			{
				Path = root,
				PackageId = "contoso.data",
				Version = "1.0.0",
				Name = "Contoso Data",
				Kind = "data",
				Force = true
			};

			await initializer.InitializeAsync(command, CancellationToken.None);

			using var package = new PacxPackageReader().Open(root);
			Assert.AreEqual("data", package.Manifest.Kind);
			Assert.AreEqual(1, package.Manifest.Artifacts.Count);
			Assert.AreEqual(1, package.Manifest.Deployment.Count);
			Assert.IsTrue(File.Exists(Path.Combine(root, "data", "records.json")));
		}
	}
}
