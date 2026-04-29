using Greg.Xrm.Command.Services;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.TestSuite;
using Moq;

namespace Greg.Xrm.Command.Commands.Tool
{
	[TestClass]
	public class UninstallCommandExecutorTest
	{
		[TestMethod]
		public async Task ExecuteAsync_ShouldMarkPluginForDeletion()
		{
			var root = TestTempPath.CreateDirectory("tool_uninstall");
			var pluginsRoot = Path.Combine(root, "Plugins");
			var pluginFolder = Path.Combine(pluginsRoot, "sample.plugin");
			Directory.CreateDirectory(pluginFolder);

			var output = new OutputToMemory();
			var storage = new Mock<IStorage>();
			storage.Setup(s => s.GetOrCreateStorageFolder()).Returns(new DirectoryInfo(root));

			var executor = new UninstallCommandExecutor(output, storage.Object);
			var result = await executor.ExecuteAsync(new UninstallCommand { Name = "sample.plugin" }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			Assert.IsTrue(File.Exists(Path.Combine(pluginFolder, ".delete")), "The uninstall command should create a deletion marker.");
			StringAssert.Contains(output.ToString(), "Deleting plugin <sample.plugin>...");
			StringAssert.Contains(output.ToString(), "Done");
		}

		[TestMethod]
		public async Task ExecuteAsync_ShouldFail_WhenPluginIsMissing()
		{
			var root = TestTempPath.CreateDirectory("tool_uninstall_missing");
			var output = new OutputToMemory();
			var storage = new Mock<IStorage>();
			storage.Setup(s => s.GetOrCreateStorageFolder()).Returns(new DirectoryInfo(root));

			var executor = new UninstallCommandExecutor(output, storage.Object);
			var result = await executor.ExecuteAsync(new UninstallCommand { Name = "missing.plugin" }, CancellationToken.None);

			Assert.IsFalse(result.IsSuccess);
			StringAssert.Contains(result.ErrorMessage, "not found");
		}
	}
}
