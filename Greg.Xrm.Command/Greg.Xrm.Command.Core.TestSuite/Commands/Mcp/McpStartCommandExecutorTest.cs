using Greg.Xrm.Command.Parsing;
using Greg.Xrm.Command.Services;
using Greg.Xrm.Command.Services.Mcp;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Mcp;

[TestClass]
public class McpStartCommandExecutorTest
{
	[TestMethod]
	public async Task ExecuteAsync_StdioShouldDelegateToLauncher()
	{
		var registry = new Mock<IReadOnlyCommandRegistry>();
		registry.SetupGet(x => x.Commands).Returns([]);

		var storage = new Mock<IStorage>();
		var output = new Mock<IOutput>();
		var launcher = new Mock<IMcpServerLauncher>();
		launcher
			.Setup(x => x.RunAsync(registry.Object, storage.Object, It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		var executor = new McpStartCommandExecutor(output.Object, registry.Object, storage.Object, launcher.Object);
		var result = await executor.ExecuteAsync(new McpStartCommand(), CancellationToken.None);

		Assert.IsTrue(result.IsSuccess);
		launcher.Verify(x => x.RunAsync(registry.Object, storage.Object, It.IsAny<CancellationToken>()), Times.Once);
	}

	[TestMethod]
	public async Task ExecuteAsync_HttpShouldFail()
	{
		var registry = new Mock<IReadOnlyCommandRegistry>();
		registry.SetupGet(x => x.Commands).Returns([]);

		var storage = new Mock<IStorage>();
		var output = new Mock<IOutput>();
		var launcher = new Mock<IMcpServerLauncher>();

		var executor = new McpStartCommandExecutor(output.Object, registry.Object, storage.Object, launcher.Object);
		var result = await executor.ExecuteAsync(new McpStartCommand { Transport = "http" }, CancellationToken.None);

		Assert.IsFalse(result.IsSuccess);
		launcher.Verify(x => x.RunAsync(It.IsAny<IReadOnlyCommandRegistry>(), It.IsAny<IStorage>(), It.IsAny<CancellationToken>()), Times.Never);
	}
}
