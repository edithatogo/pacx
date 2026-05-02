using Greg.Xrm.Command.Services;

namespace Greg.Xrm.Command.Commands.Tool
{
	[TestClass]
	public class SourceRemoveCommandExecutorTest
	{
		[TestMethod]
		public void RemoveNonExistentShouldFail()
		{
			var tempDir = TestTempPath.CreateDirectory("source_remove_test");
			try
			{
				var storageMock = new Mock<IStorage>();
				Directory.CreateDirectory(Path.Combine(tempDir, "Sources"));
				storageMock.Setup(s => s.GetOrCreateStorageFolder()).Returns(new DirectoryInfo(tempDir));

				var output = new OutputToMemory();
				var executor = new SourceRemoveCommandExecutor(output, storageMock.Object);

				var result = executor.ExecuteAsync(
					new SourceRemoveCommand { Name = "nonexistent" },
					CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsFalse(result.IsSuccess);
				StringAssert.Contains(result.ErrorMessage, "not registered");
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}
	}
}
