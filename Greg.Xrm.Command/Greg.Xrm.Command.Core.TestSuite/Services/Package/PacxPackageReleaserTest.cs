using System.Text.Json;
using Greg.Xrm.Command.Commands.Package;
using Greg.Xrm.Command.TestSuite;

namespace Greg.Xrm.Command.Services.Package
{
	[TestClass]
	public class PacxPackageReleaserTest
	{
		[TestMethod]
		public async Task ReleaseShouldStageFolderWithNotesAndChecksums()
		{
			var root = TestTempPath.CreateDirectory("pacx_release");
			var destination = TestTempPath.CreateDirectory("pacx_release_out");

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
				File.WriteAllBytes(Path.Combine(root, "payload", "solution.zip"), new byte[] { 1, 2, 3, 4 });

				var authoring = new PacxPackageAuthoringService();
				await authoring.SyncAsync(new PackageSyncCommand { Path = root }, CancellationToken.None);

				var releaser = new PacxPackageReleaser(new PacxPackageReader(), new PacxPackagePublisher(new PacxPackageReader(), new PacxPackageBuilder(new PacxPackageReader())));
				var result = await releaser.ReleaseAsync(new PackageReleaseCommand
				{
					Path = root,
					DestinationPath = destination,
					Version = "2.0.0"
				}, CancellationToken.None);

				Assert.AreEqual(Path.Combine(destination, "contoso.sample.2.0.0"), result.ReleaseDirectory);
				Assert.IsTrue(Directory.Exists(result.ReleaseDirectory));
				Assert.IsTrue(File.Exists(result.PackagePath));
				Assert.IsTrue(File.Exists(result.ReleaseManifestPath));
				Assert.IsTrue(File.Exists(result.ProvenancePath));
				Assert.IsTrue(File.Exists(result.SbomPath));
				Assert.IsTrue(File.Exists(result.ReleaseNotesPath));
				Assert.IsTrue(File.Exists(result.ChecksumsPath));

				var release = JsonSerializer.Deserialize<PacxPackageReleaseManifest>(File.ReadAllText(result.ReleaseManifestPath));
				Assert.IsNotNull(release);
				Assert.AreEqual("contoso.sample", release!.PackageId);
				Assert.AreEqual("2.0.0", release.Version);
				Assert.AreEqual(Path.Combine(result.ReleaseDirectory, "contoso.sample.2.0.0.pacx"), result.PackagePath);
				StringAssert.Contains(File.ReadAllText(result.ReleaseNotesPath), "Contoso Sample");
				StringAssert.Contains(File.ReadAllText(result.ReleaseNotesPath), "Version: 2.0.0");
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
	}
}
