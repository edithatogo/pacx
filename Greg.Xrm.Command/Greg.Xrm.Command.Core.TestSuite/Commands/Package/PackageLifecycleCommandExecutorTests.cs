using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Package;

namespace Greg.Xrm.Command.Commands.Package
{
	[TestClass]
	public class PackageLifecycleCommandExecutorTests
	{
		[TestMethod]
		public async Task InitExecutorShouldWriteSuccessMessage()
		{
			var output = new OutputToMemory();
			var initializer = new Mock<IPacxPackageInitializer>();
			initializer.Setup(x => x.InitializeAsync(It.IsAny<PackageInitCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync("./package");

			var executor = new PackageInitCommandExecutor(output, initializer.Object);
			var result = await executor.ExecuteAsync(new PackageInitCommand { Path = "./package" }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			StringAssert.Contains(output.ToString(), "Initialized PACX package at ./package");
			initializer.Verify(x => x.InitializeAsync(It.IsAny<PackageInitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task BuildExecutorShouldWriteSuccessMessage()
		{
			var output = new OutputToMemory();
			var builder = new Mock<IPacxPackageBuilder>();
			builder.Setup(x => x.BuildAsync("./package", null, It.IsAny<CancellationToken>())).ReturnsAsync("./package/contoso.sample.1.0.0.pacx");

			var executor = new PackageBuildCommandExecutor(output, builder.Object);
			var result = await executor.ExecuteAsync(new PackageBuildCommand { Path = "./package" }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			StringAssert.Contains(output.ToString(), "Built PACX package: ./package/contoso.sample.1.0.0.pacx");
			builder.Verify(x => x.BuildAsync("./package", null, It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task PublishExecutorShouldWriteSuccessMessage()
		{
			var output = new OutputToMemory();
			var publisher = new Mock<IPacxPackagePublisher>();
			publisher.Setup(x => x.PublishAsync(It.IsAny<PackagePublishCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new PacxPackagePublishResult(
					"./releases/contoso.sample.1.0.0.pacx",
					"./releases/pacx.release.json",
					"abc123"));

			var executor = new PackagePublishCommandExecutor(output, publisher.Object);
			var result = await executor.ExecuteAsync(new PackagePublishCommand { Path = "./package", DestinationPath = "./releases" }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			StringAssert.Contains(output.ToString(), "Published PACX archive: ./releases/contoso.sample.1.0.0.pacx");
			StringAssert.Contains(output.ToString(), "Release manifest: ./releases/pacx.release.json");
			StringAssert.Contains(output.ToString(), "SHA256: abc123");
			publisher.Verify(x => x.PublishAsync(It.IsAny<PackagePublishCommand>(), It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task ReleaseExecutorShouldWriteSuccessMessage()
		{
			var output = new OutputToMemory();
			var releaser = new Mock<IPacxPackageReleaser>();
			releaser.Setup(x => x.ReleaseAsync(It.IsAny<PackageReleaseCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new PacxPackageReleaseResult(
					"./releases/contoso.sample.1.0.0",
					"./releases/contoso.sample.1.0.0/contoso.sample.1.0.0.pacx",
					"./releases/contoso.sample.1.0.0/pacx.release.json",
					"./releases/contoso.sample.1.0.0/provenance.json",
					"./releases/contoso.sample.1.0.0/sbom.json",
					"./releases/contoso.sample.1.0.0/RELEASE_NOTES.md",
					"./releases/contoso.sample.1.0.0/checksums.txt"));

			var executor = new PackageReleaseCommandExecutor(output, releaser.Object);
			var result = await executor.ExecuteAsync(new PackageReleaseCommand { Path = "./package" }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			StringAssert.Contains(output.ToString(), "Staged PACX release: ./releases/contoso.sample.1.0.0");
			StringAssert.Contains(output.ToString(), "Package: ./releases/contoso.sample.1.0.0/contoso.sample.1.0.0.pacx");
			StringAssert.Contains(output.ToString(), "Manifest: ./releases/contoso.sample.1.0.0/pacx.release.json");
			StringAssert.Contains(output.ToString(), "Provenance: ./releases/contoso.sample.1.0.0/provenance.json");
			StringAssert.Contains(output.ToString(), "SBOM: ./releases/contoso.sample.1.0.0/sbom.json");
			releaser.Verify(x => x.ReleaseAsync(It.IsAny<PackageReleaseCommand>(), It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task ShowExecutorShouldWritePackageSummary()
		{
			var output = new OutputToMemory();
			var package = new Mock<IPacxPackageSource>();
			package.SetupGet(x => x.Manifest).Returns(new PacxPackageManifest
			{
				PackageId = "contoso.sample",
				Version = "1.0.0",
				Name = "Contoso Sample",
				Kind = "bundle",
				SchemaVersion = 1
			});
			package.SetupGet(x => x.Entries).Returns(new List<PacxPackageEntry>
			{
				new() { Path = "payload/solution.zip", Length = 4 }
			});
			package.Setup(x => x.Dispose());

			var reader = new Mock<IPacxPackageReader>();
			reader.Setup(x => x.Open("./package")).Returns(package.Object);

			var executor = new PackageShowCommandExecutor(output, reader.Object);
			var result = await executor.ExecuteAsync(new PackageShowCommand { Path = "./package" }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			StringAssert.Contains(output.ToString(), "PACX package: Contoso Sample");
			StringAssert.Contains(output.ToString(), "Kind: bundle");
			StringAssert.Contains(output.ToString(), "payload/solution.zip (4 bytes)");
		}

		[TestMethod]
		public async Task DeployExecutorShouldWriteLoadedPackageMessageAndInvokeDeployer()
		{
			var output = new OutputToMemory();
			var package = new Mock<IPacxPackageSource>();
			package.SetupGet(x => x.Manifest).Returns(new PacxPackageManifest
			{
				PackageId = "contoso.sample",
				Version = "1.0.0",
				Name = "Contoso Sample",
				Kind = "bundle",
				SchemaVersion = 1
			});
			package.Setup(x => x.Dispose());

			var reader = new Mock<IPacxPackageReader>();
			reader.Setup(x => x.Open("./package")).Returns(package.Object);

			var deployer = new Mock<IPacxPackageDeployer>();
			deployer.Setup(x => x.DeployAsync(package.Object, true, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

			var executor = new PackageDeployCommandExecutor(output, reader.Object, deployer.Object);
			var result = await executor.ExecuteAsync(new PackageDeployCommand { Path = "./package", DryRun = true }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			StringAssert.Contains(output.ToString(), "Loaded PACX package contoso.sample 1.0.0");
			reader.Verify(x => x.Open("./package"), Times.Once);
			deployer.Verify(x => x.DeployAsync(package.Object, true, It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}
