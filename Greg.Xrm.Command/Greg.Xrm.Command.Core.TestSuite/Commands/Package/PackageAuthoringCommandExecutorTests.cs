using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Package;

namespace Greg.Xrm.Command.Commands.Package
{
	[TestClass]
	public class PackageAuthoringCommandExecutorTests
	{
		[TestMethod]
		public async Task AddSolutionExecutorShouldWriteSuccessMessage()
		{
			var output = new OutputToMemory();
			var authoring = new Mock<IPacxPackageAuthoringService>();
			authoring
				.Setup(x => x.AddSolutionAsync(It.IsAny<PackageAddSolutionCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync("payload/solution.zip");

			var executor = new PackageAddSolutionCommandExecutor(output, authoring.Object);
			var result = await executor.ExecuteAsync(new PackageAddSolutionCommand { Path = "./package" }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			StringAssert.Contains(output.ToString(), "Added solution payload to package: payload/solution.zip");
			authoring.Verify(x => x.AddSolutionAsync(It.IsAny<PackageAddSolutionCommand>(), It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task AddDataExecutorShouldWriteSuccessMessage()
		{
			var output = new OutputToMemory();
			var authoring = new Mock<IPacxPackageAuthoringService>();
			authoring
				.Setup(x => x.AddDataAsync(It.IsAny<PackageAddDataCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync("data/accounts.json");

			var executor = new PackageAddDataCommandExecutor(output, authoring.Object);
			var result = await executor.ExecuteAsync(new PackageAddDataCommand { Path = "./package" }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			StringAssert.Contains(output.ToString(), "Added data payload to package: data/accounts.json");
			authoring.Verify(x => x.AddDataAsync(It.IsAny<PackageAddDataCommand>(), It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task RemoveSolutionExecutorShouldWriteSuccessMessage()
		{
			var output = new OutputToMemory();
			var authoring = new Mock<IPacxPackageAuthoringService>();
			authoring
				.Setup(x => x.RemoveSolutionAsync(It.IsAny<PackageRemoveSolutionCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync("payload/solution.zip");

			var executor = new PackageRemoveSolutionCommandExecutor(output, authoring.Object);
			var result = await executor.ExecuteAsync(new PackageRemoveSolutionCommand { Path = "./package" }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			StringAssert.Contains(output.ToString(), "Removed solution payload from package: payload/solution.zip");
			authoring.Verify(x => x.RemoveSolutionAsync(It.IsAny<PackageRemoveSolutionCommand>(), It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task RemoveDataExecutorShouldWriteSuccessMessage()
		{
			var output = new OutputToMemory();
			var authoring = new Mock<IPacxPackageAuthoringService>();
			authoring
				.Setup(x => x.RemoveDataAsync(It.IsAny<PackageRemoveDataCommand>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync("data/accounts.json");

			var executor = new PackageRemoveDataCommandExecutor(output, authoring.Object);
			var result = await executor.ExecuteAsync(new PackageRemoveDataCommand { Path = "./package" }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			StringAssert.Contains(output.ToString(), "Removed data payload from package: data/accounts.json");
			authoring.Verify(x => x.RemoveDataAsync(It.IsAny<PackageRemoveDataCommand>(), It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}
