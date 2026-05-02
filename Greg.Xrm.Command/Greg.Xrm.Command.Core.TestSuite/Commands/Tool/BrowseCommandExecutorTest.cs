using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Tool
{
	[TestClass]
	public class BrowseCommandExecutorTest
	{
		[TestMethod]
		public void ExecuteShouldRenderMatchingTools()
		{
			var tempDir = TestTempPath.CreateDirectory("tool_browse_test");
			var catalogPath = Path.Combine(tempDir, "tools.json");
			try
			{
				File.WriteAllText(catalogPath, """{ "tools": [{"id":"test-tool","name":"Test Tool","provider":"PACX","category":"core","kind":"cli","summary":"A test tool.","capabilities":["test"]}] }""");

				var output = new OutputToMemory();
				var executor = new BrowseCommandExecutor(output);
				var result = executor.ExecuteAsync(
					new BrowseCommand { CatalogPath = catalogPath },
					CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				StringAssert.Contains(output.ToString(), "Test Tool");
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}
	}
}
