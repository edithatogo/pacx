using Greg.Xrm.Command.Services;

namespace Greg.Xrm.Command.Commands.Tool
{
	[TestClass]
	public class SourceListCommandExecutorTest
	{
		[TestMethod]
		public void ListEmptyShouldShowNoSources()
		{
			var tempDir = TestTempPath.CreateDirectory("source_list_empty_test");
			try
			{
				var storageMock = new Mock<IStorage>();
				Directory.CreateDirectory(Path.Combine(tempDir, "Sources"));
				storageMock.Setup(s => s.GetOrCreateStorageFolder()).Returns(new DirectoryInfo(tempDir));

				var output = new OutputToMemory();
				var executor = new SourceListCommandExecutor(output, storageMock.Object);

				var result = executor.ExecuteAsync(
					new SourceListCommand(),
					CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}
	}
}
