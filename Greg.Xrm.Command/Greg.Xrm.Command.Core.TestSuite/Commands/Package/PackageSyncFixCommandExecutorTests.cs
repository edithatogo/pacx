using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Package;

namespace Greg.Xrm.Command.Commands.Package
{
	[TestClass]
	public class PackageSyncFixCommandExecutorTests
	{
		[TestMethod]
		public async Task SyncExecutorShouldWriteSummaryMessage()
		{
			var output = new OutputToMemory();
			var authoring = new Mock<IPacxPackageAuthoringService>();
			authoring
				.Setup(x => x.SyncAsync(It.IsAny<PackageSyncCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new PacxPackageSyncResult(3, 2, 1, 0));

			var executor = new PackageSyncCommandExecutor(output, authoring.Object);
			var result = await executor.ExecuteAsync(new PackageSyncCommand { Path = "./package" }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			StringAssert.Contains(output.ToString(), "Synchronized package folder: +3 artifacts, +2 steps, -1 artifacts, -0 steps");
			authoring.Verify(x => x.SyncAsync(It.IsAny<PackageSyncCommand>(), It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task FixExecutorShouldWriteSummaryMessage()
		{
			var output = new OutputToMemory();
			var authoring = new Mock<IPacxPackageAuthoringService>();
			authoring
				.Setup(x => x.FixAsync(It.IsAny<PackageFixCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new PacxPackageFixResult(1, 2, 3, 4, 5, 6));

			var executor = new PackageFixCommandExecutor(output, authoring.Object);
			var result = await executor.ExecuteAsync(new PackageFixCommand { Path = "./package" }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			StringAssert.Contains(output.ToString(), "Fixed package folder: +1 artifacts, +2 steps, -3 duplicate artifacts, -4 duplicate steps, -5 stale artifacts, -6 stale steps");
			authoring.Verify(x => x.FixAsync(It.IsAny<PackageFixCommand>(), It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}
