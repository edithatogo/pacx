using Greg.Xrm.Command.Services;

namespace Greg.Xrm.Command.Commands.Tool
{
	[TestClass]
	public class SourceAddCommandExecutorTest
	{
		[TestMethod]
		public void AddShouldCreateSourceFile()
		{
			var tempDir = TestTempPath.CreateDirectory("source_add_test");
			try
			{
				var storageMock = new Mock<IStorage>();
				var sourcesDir = Directory.CreateDirectory(Path.Combine(tempDir, "Sources"));
				storageMock.Setup(s => s.GetOrCreateStorageFolder()).Returns(new DirectoryInfo(tempDir));

				var output = new OutputToMemory();
				var executor = new SourceAddCommandExecutor(output, storageMock.Object);

				var result = executor.ExecuteAsync(
					new SourceAddCommand { Name = "test-feed", Url = "https://example.com", Type = "nuget" },
					CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);

				var sourcePath = Path.Combine(tempDir, "Sources", "test-feed.json");
				Assert.IsTrue(File.Exists(sourcePath));
				StringAssert.Contains(output.ToString(), "test-feed");
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}

		[TestMethod]
		public void AddDuplicateShouldFail()
		{
			var tempDir = TestTempPath.CreateDirectory("source_add_dup_test");
			try
			{
				var storageMock = new Mock<IStorage>();
				Directory.CreateDirectory(Path.Combine(tempDir, "Sources"));
				storageMock.Setup(s => s.GetOrCreateStorageFolder()).Returns(new DirectoryInfo(tempDir));

				var output = new OutputToMemory();
				var executor = new SourceAddCommandExecutor(output, storageMock.Object);

				executor.ExecuteAsync(
					new SourceAddCommand { Name = "dup-feed", Url = "https://example.com" },
					CancellationToken.None).GetAwaiter().GetResult();

				var result = executor.ExecuteAsync(
					new SourceAddCommand { Name = "dup-feed", Url = "https://other.com" },
					CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsFalse(result.IsSuccess);
				StringAssert.Contains(result.ErrorMessage, "already registered");
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}
	}
}
