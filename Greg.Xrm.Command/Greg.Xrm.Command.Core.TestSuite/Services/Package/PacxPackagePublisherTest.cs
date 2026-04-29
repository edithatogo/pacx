using System.IO.Compression;
using System.Text.Json;
using Greg.Xrm.Command.Commands.Package;
using Greg.Xrm.Command.TestSuite;

namespace Greg.Xrm.Command.Services.Package
{
	[TestClass]
	public class PacxPackagePublisherTest
	{
		[TestMethod]
		public async Task PublishShouldBuildFolderAndWriteReleaseManifest()
		{
			var root = TestTempPath.CreateDirectory("pacx_publish");
			var destination = TestTempPath.CreateDirectory("pacx_publish_out");

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

				var publisher = new PacxPackagePublisher(new PacxPackageReader(), new PacxPackageBuilder(new PacxPackageReader()));
				var result = await publisher.PublishAsync(new PackagePublishCommand
				{
					Path = root,
					DestinationPath = destination
				}, CancellationToken.None);

				Assert.IsTrue(File.Exists(result.PackagePath));
				Assert.IsTrue(File.Exists(result.ReleaseManifestPath));
				Assert.AreEqual(Path.Combine(destination, "contoso.sample.1.0.0.pacx"), result.PackagePath);

				var release = JsonSerializer.Deserialize<PacxPackageReleaseManifest>(File.ReadAllText(result.ReleaseManifestPath));
				Assert.IsNotNull(release);
				Assert.AreEqual("contoso.sample", release!.PackageId);
				Assert.AreEqual("1.0.0", release.Version);
				Assert.AreEqual(result.PackagePath, release.ArchivePath);
				Assert.AreEqual(result.ArchiveSha256, release.ArchiveSha256);
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
		public async Task PublishShouldHonorVersionOverride()
		{
			var root = TestTempPath.CreateDirectory("pacx_publish_version");
			var destination = TestTempPath.CreateDirectory("pacx_publish_version_out");

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

				var publisher = new PacxPackagePublisher(new PacxPackageReader(), new PacxPackageBuilder(new PacxPackageReader()));
				var result = await publisher.PublishAsync(new PackagePublishCommand
				{
					Path = root,
					DestinationPath = destination,
					Version = "2.0.0"
				}, CancellationToken.None);

				Assert.AreEqual(Path.Combine(destination, "contoso.sample.2.0.0.pacx"), result.PackagePath);

				var release = JsonSerializer.Deserialize<PacxPackageReleaseManifest>(File.ReadAllText(result.ReleaseManifestPath));
				Assert.IsNotNull(release);
				Assert.AreEqual("2.0.0", release!.Version);

				using var publishedPackage = new PacxPackageReader().Open(result.PackagePath);
				Assert.AreEqual("2.0.0", publishedPackage.Manifest.Version);
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
		public async Task PublishShouldCopyExistingArchive()
		{
			var root = TestTempPath.CreateDirectory("pacx_publish_archive");
			var builtArchiveRoot = TestTempPath.CreateDirectory("pacx_publish_archive_built");
			var destination = TestTempPath.CreateDirectory("pacx_publish_archive_out");

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

				var builtArchive = Path.Combine(builtArchiveRoot, "contoso.sample.1.0.0.pacx");
				ZipFile.CreateFromDirectory(root, builtArchive, CompressionLevel.Optimal, includeBaseDirectory: false);

				var publisher = new PacxPackagePublisher(new PacxPackageReader(), new PacxPackageBuilder(new PacxPackageReader()));
				var result = await publisher.PublishAsync(new PackagePublishCommand
				{
					Path = builtArchive,
					DestinationPath = destination
				}, CancellationToken.None);

				Assert.IsTrue(File.Exists(result.PackagePath));
				Assert.IsTrue(File.Exists(result.ReleaseManifestPath));
				Assert.AreEqual(Path.Combine(destination, "contoso.sample.1.0.0.pacx"), result.PackagePath);
			}
			finally
			{
				if (Directory.Exists(root))
				{
					Directory.Delete(root, true);
				}
				if (Directory.Exists(builtArchiveRoot))
				{
					Directory.Delete(builtArchiveRoot, true);
				}
				if (Directory.Exists(destination))
				{
					Directory.Delete(destination, true);
				}
			}
		}

		[TestMethod]
		public async Task PublishShouldHonorVersionOverrideForExistingArchive()
		{
			var root = TestTempPath.CreateDirectory("pacx_publish_archive_version");
			var builtArchiveRoot = TestTempPath.CreateDirectory("pacx_publish_archive_version_built");
			var destination = TestTempPath.CreateDirectory("pacx_publish_archive_version_out");

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

				var builtArchive = Path.Combine(builtArchiveRoot, "contoso.sample.1.0.0.pacx");
				ZipFile.CreateFromDirectory(root, builtArchive, CompressionLevel.Optimal, includeBaseDirectory: false);

				var publisher = new PacxPackagePublisher(new PacxPackageReader(), new PacxPackageBuilder(new PacxPackageReader()));
				var result = await publisher.PublishAsync(new PackagePublishCommand
				{
					Path = builtArchive,
					DestinationPath = destination,
					Version = "2.0.0"
				}, CancellationToken.None);

				Assert.AreEqual(Path.Combine(destination, "contoso.sample.2.0.0.pacx"), result.PackagePath);

				var release = JsonSerializer.Deserialize<PacxPackageReleaseManifest>(File.ReadAllText(result.ReleaseManifestPath));
				Assert.IsNotNull(release);
				Assert.AreEqual("2.0.0", release!.Version);

				using var publishedPackage = new PacxPackageReader().Open(result.PackagePath);
				Assert.AreEqual("2.0.0", publishedPackage.Manifest.Version);
			}
			finally
			{
				if (Directory.Exists(root))
				{
					Directory.Delete(root, true);
				}
				if (Directory.Exists(builtArchiveRoot))
				{
					Directory.Delete(builtArchiveRoot, true);
				}
				if (Directory.Exists(destination))
				{
					Directory.Delete(destination, true);
				}
			}
		}
	}
}
